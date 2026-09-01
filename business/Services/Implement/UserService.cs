using api.application;
using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using business.mapper;
using api.util;
using business.Services.Interface;
using data.dto.Request;
using data.dto.Response;
using data.util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace business.Services.Implement;

public class UserService(
    IConfiguration config,
    IFileServices fileServices,
    IUnitOfWork unitOfWork,
    IAuthenticationService authenticationService,
    IServiceProvider sp,
    HybridCache cache,
    ILogger<UserService> logger
) : IUserServices
{
    public async Task<Result> Signup(SignupDto signupDto)
    {
        logger.LogInformation("start user signup");

        var validationResult = ClsValidation.ValidateInput(signupDto.Email, signupDto.Password, signupDto.Phone);

        if (validationResult != null)
        {
            logger.LogError("signup validation input error with {errorMessage}", validationResult);
            return new Result(false, validationResult, null, 400);
        }

        var isExistByEmail = await unitOfWork.UserRepository.IsExistByEmail(signupDto.Email);
        if (isExistByEmail)
        {
            logger.LogError("email already exist");
            return new Result(false, "email already exist", null, 409);
        }

        var isExistByPhone = await unitOfWork.UserRepository.IsExistByPhone(signupDto.Phone);
        if (isExistByPhone)
        {
            logger.LogError("phone already exist");
            return new Result(false, "phone already exist", null,409);
        }

        if (signupDto.Role == 0 && await unitOfWork.UserRepository.IsExist(false))
        {
            logger.LogError("you cannot create a user with exist role");
            return new Result(false, "you cannot create a user with exist role", null,  403);
        }

        var userId = ClsUtil.GenerateGuid();
        var user = new User
        {
            Id = userId,
            Name = signupDto.Name,
            Phone = signupDto.Phone,
            Password = ClsUtil.HashingText(signupDto.Password),
            IsUser = (signupDto.Role ?? EnRole.User) == EnRole.User,
            DeviceToken = signupDto.DeviceToken ?? "",
            Thumbnail = "",
            CreatedAt = DateTime.Now,
            Email = signupDto.Email,
            UpdatedAt = null,
        };

        unitOfWork.UserRepository.Add(user);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("there are error in create new user");
            return new Result(false, "there are error in create new user", null, 500);
        }

        var tokenData = await authenticationService.GenerateToken(id: userId, email: signupDto.Email, [EnUserType.User]);

        logger.LogInformation("end user signup");
        return new Result(true, null, tokenData, 200);
    }

    public async Task<Result> Login(LoginDto loginDto)
    {
        logger.LogInformation("start user login");

        var user = await unitOfWork.UserRepository.GetUser(loginDto.Username, ClsUtil.HashingText(loginDto.Password));
        var validationResult = user.IsValidateFunc(false);
        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", user?.Id, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        user!.DeviceToken = loginDto.DeviceToken;
        unitOfWork.UserRepository.Update(user);

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("Error While Login Use");
            return new Result(false, "Error While Login User", null, 500);
        }

        var userRefreshTokenHolder = await unitOfWork.UserRefreshTokenRepository.GetByUserId(user!.Id);

        var role = userRefreshTokenHolder!.Role switch
        {
            "Admin" => EnUserType.Admin,
            "Delivery" => EnUserType.Delivery,
            _ => EnUserType.User
        };

        var tokenData = await authenticationService.GenerateToken(id: user!.Id, email: user.Email, [role]);

        logger.LogInformation("end user login");
        return new Result(true, null, tokenData, 200);
    }

    public async Task<Result> GetMe(Guid id)
    {
        logger.LogInformation("start get user info");

        var user = await unitOfWork.UserRepository.GetUser(id);
        var validationResult = user.IsValidateFunc(false);
        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", user?.Id, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var userToDto = user!.ToUserInfoDto(config["url_file"] ?? "");

        logger.LogInformation("end get user info");
        return new Result(true, null, userToDto, 200);
    }

    public async Task<Result> GetUsers(int page, Guid id)
    {
        logger.LogInformation("start getting users by page");

        var user = await unitOfWork.UserRepository.GetUser(id);
        var validationResult = user.IsValidateFunc();
        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", user?.Id, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var users = await cache.GetOrCreateAsync(MemoryCacheKeys.UsersKey + "/" + id + "/" + page,
            async ct =>
            {
                var users = (await unitOfWork.UserRepository.GetUsers(page, 25))
                    .Select(u => u.ToUserInfoDto(config["url_file"] ?? ""))
                    .ToList();
                return users;
            },
            tags: [MemoryCacheKeys.UsersKey]);

        logger.LogInformation("end getting users by page");
        return new Result(true, null, users, 200);
    }

    public async Task<Result> GetUsersPages(Guid id, int pageLenght)
    {
        logger.LogInformation("start getting users page");

        var user = await unitOfWork.UserRepository.GetUser(id);
        var validationResult = user.IsValidateFunc();
        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", user?.Id, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var userPages = await unitOfWork.UserRepository.GetUserCount();
        var pageUserCount = userPages > 0 ? (int)Math.Ceiling((double)userPages / pageLenght) : 0;

        logger.LogInformation("end getting users page");
        return new Result(true, null, pageUserCount, 200);
    }

    public async Task<Result> BlockOrUnBlockUser(Guid id, Guid userId)
    {
        logger.LogInformation("start chanage user Status ById");

        var admin = await unitOfWork.UserRepository.GetUser(id);
        var validationResult = admin.IsValidateFunc();
        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", userId, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var user = await unitOfWork.UserRepository.GetUser(userId);
        validationResult = user.IsValidateFunc();

        if (validationResult is not null)
        {
            logger.LogError("could not change {userId} status to {statns}", userId, (user?.IsBlocked == true ? "block" : "unblock"));
            return new Result(false, $"unable to {(user?.IsBlocked == true ? "block" : "unblock")}  user", null, 403);
        }

        user!.IsBlocked = !user.IsBlocked;

        if (user is { IsBlocked: true, IsUser: false })
        {
            logger.LogError("could not blockk admin user");
            return new Result(false, "you could not block admin user ", null,403);
        }

        unitOfWork.UserRepository.Update(user);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("error whiel change {userId} status to {statns}", userId, (user?.IsBlocked == true ? "block" : "unblock"));
            return new Result(false, "error while change user Blocking status", null, 500);
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.UsersKey);
        logger.LogInformation("end chanage user Status ById");
        return new Result(true, null, null, 204);
    }

    public async Task<Result> UpdateUser(UpdateUserInfoDto userDto, Guid id,string rootPath, bool isUpdateWillBeTop = false)
    {
        logger.LogInformation("start update user info");

        if (userDto.IsEmpty())
        {
            logger.LogError("no user change found");
            return new Result(false, "no data changes", null, 400);
        }

        var user = await unitOfWork.UserRepository.GetUser(id);
        var validationResult = user.IsValidateFunc(false);

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", user?.Id, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        if (userDto.Phone is not null && user?.Phone != userDto.Phone)
        {
            var isExistPhone = await unitOfWork.UserRepository.IsExistByPhone(userDto.Phone ?? "");
            if (isExistPhone)
            {
                logger.LogError("phone already exist");
                return new Result(false, "phone already exist", null, 409);
            }
        }

        var hashedPassword = string.IsNullOrEmpty(userDto.Password) || string.IsNullOrEmpty(userDto.NewPassword)
            ? null
            : ClsUtil.HashingText(userDto.NewPassword);

        if (userDto is { Password: not null, NewPassword: not null })
        {
            if (user?.Password != ClsUtil.HashingText(userDto.Password))
            {
                logger.LogError("envalid previuse password for {userId}", user?.Id);
                return new Result(false, "Enter Valid Previous Password", null,  409 );
            }
        }

        string? profile = null;
        if (userDto.Thumbnail != null)
        {
            profile = await fileServices.SaveFile(userDto.Thumbnail, EnImageType.Profile,rootPath);
        }

        user?.Thumbnail = profile ?? user.Thumbnail;
        user?.Name = userDto.Name ?? user.Name;
        user?.Phone = userDto.Phone ?? user.Phone;
        user?.UpdatedAt = DateTime.Now;
        user?.Password = hashedPassword ?? user.Password;

        unitOfWork.UserRepository.Update(user!);

        if (isUpdateWillBeTop)
        {
            return new Result(true, null, null, 204);
        }

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("error while updating user");
            return new Result(false, "error while updating user", null, 500);
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.UsersKey);
        logger.LogInformation("end update user info");
        return new Result(true, null, null, 204);
    }

    public async Task<Result> AddAddressToUser(CreateAddressDto addressDto, Guid id)
    {
        logger.LogInformation("start add new Address To User");

        var user = await unitOfWork.UserRepository.GetUser(id);
        var validationResult = user.IsValidateFunc(false);

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", user?.Id, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var addressCount = await unitOfWork.AddressRepository.GetAddressCount(id);

        if (addressCount == 20)
        {
            logger.LogError("user {userId} hit the limit of 20 address can saved", user?.Id);
            return new Result(false, "maximum 20 addresses reached", null, 403);
        }

        var address = new Address
        {
            Id = ClsUtil.GenerateGuid(),
            Longitude = addressDto.Longitude,
            Latitude = addressDto.Latitude,
            Title = addressDto.Title,
            OwnerId = user!.Id,
            IsCurrent = true
        };

        unitOfWork.AddressRepository.MakeAddressNotCurrentToId(user.Id);
        unitOfWork.AddressRepository.Add(address);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("error while adding address");
            return new Result(false, "error while adding address", null, 500);
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.UsersKey);
        logger.LogInformation("end add new Address To User");
        return new Result(true, null, address.ToDto(), 201);
    }

    public async Task<Result> UpdateUserAddress(UpdateAddressDto addressDto, Guid id)
    {
        logger.LogInformation("start update user Address");

        if (addressDto.IsEmpty())
        {
            return new Result(false, "nothing to be updated", null, 400);
        }

        var user = await unitOfWork.UserRepository.GetUser(id);
        var validationResult = user.IsValidateFunc(false);
        if (validationResult is not null)
        {
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        if ((addressDto.Longitude is null && addressDto.Latitude is not null) ||
            (addressDto.Longitude is not null && addressDto.Latitude is null))
        {
            return new Result(false, "when update address you must change both longitude and latitude not one of them only ", null, 400);
        }

        var address = await unitOfWork.AddressRepository.GetAddress(addressDto.Id);
        if (address is null)
        {
            return new Result(false, "address not found", null, 404);
        }

        if (address.OwnerId != id)
        {
            return new Result(false, "address not belong to you", null, 404);
        }

        address.Longitude = addressDto.Longitude ?? address.Longitude;
        address.Title = addressDto.Title ?? address.Title;
        address.Latitude = addressDto.Latitude ?? address.Latitude;

        unitOfWork.AddressRepository.Update(address);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new Result(false, "error while updating address", null, 500);
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.UsersKey);
        return new Result(true, null, null, 204);
    }

    public async Task<Result> DeleteUserAddress(Guid addressId, Guid id)
    {
        logger.LogInformation("start deleting user address");
        var user = await unitOfWork.UserRepository.GetUser(id);
        var validationResult = user.IsValidateFunc(false, isStore: true);

        if (validationResult is not null)
        {
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var address = await unitOfWork.AddressRepository.GetAddress(addressId);
        if (address is null)
        {
            return new Result(false, "address not found", null, 404);
        }

        if (address.OwnerId != id)
        {
            return new Result(false, "address not belong to you", null, 404);
        }

        if (address.IsCurrent)
        {
            return new Result(false, "could not delete current address", null, 500);
        }

        unitOfWork.AddressRepository.Delete(addressId);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new Result(false, "error while delete address", null, 500);
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.UsersKey);
        logger.LogInformation("end deleting user address");
        return new Result(true, null, null, 204);
    }

    public async Task<Result> UpdateUserCurrentAddress(Guid addressId, Guid id)
    {
        logger.LogInformation("start update user current address");

        var user = await unitOfWork.UserRepository.GetUser(id);
        var validationResult = user.IsValidateFunc(false);

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {status}", user?.Id, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var address = await unitOfWork.AddressRepository.GetAddress(addressId);
        if (address is null)
        {
            logger.LogError("address not found for {addressId}", addressId);
            return new Result(false, "address not found", null, 404);
        }

        if (address.OwnerId != id)
        {
            logger.LogError("address {addressId} does not belong to user {userId}", addressId, id);
            return new Result(false, "address not belong to you", null, 404);
        }

        if (address.IsCurrent)
        {
            logger.LogError("address {addressId} is already current for user {userId}", addressId, id);
            return new Result(false, "address is already current address", null, 409);
        }

        address.IsCurrent = true;
        
      await  unitOfWork.AddressRepository.MakeAddressNotCurrentToId(user!.Id);
        unitOfWork.AddressRepository.Update(address);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("error while update current address for {addressId} and user {userId}", addressId, user.Id);
            return new Result(false, "error while update current address", null, 500);
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.UsersKey);
        logger.LogInformation("end update user current address");
        return new Result(true, null, null, 204);
    }

    public async Task<Result> GenerateOtp(ForgetPasswordDto forgetPasswordDto)
    {
        logger.LogInformation("start generate otp for {email}", forgetPasswordDto.Email);

        var user = await unitOfWork.UserRepository.GetUser(forgetPasswordDto.Email);
        var validationResult = user.IsValidateFunc(false);

        if (validationResult is not null)
        {
            logger.LogError("user not valid for forget password {email} validationError {status}", forgetPasswordDto.Email, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var otp = ClsUtil.GenerateGuid().ToString().Substring(0, 6).Replace("-", "");
        var isOtpExist = await unitOfWork.PasswordRepository.IsExist(otp, user!.Email);
        var isExist = isOtpExist;

        if (isExist)
        {
            do
            {
                otp = ClsUtil.GenerateGuid().ToString().Substring(0, 6).Replace("-", "");
                isOtpExist = await unitOfWork.PasswordRepository.IsExist(otp, user!.Email);
            } while (isOtpExist);
        }

        unitOfWork.PasswordRepository.Add(new ReseatPasswordOtp
        {
            Email = forgetPasswordDto.Email,
            CreatedAt = DateTime.Now.AddHours(1),
            Id = ClsUtil.GenerateGuid(),
            Otp = otp
        });

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("error while generate otp for {email}", forgetPasswordDto.Email);
            return new Result(false, "error while generate otp", null, 500);
        }

        var sendMessageService = sp.GetRequiredKeyedService<IMessageService>(EnMessageService.Email);
        var emailSendResult = await sendMessageService.SendingMessage(message: otp, otp);

        if (!emailSendResult)
        {
            logger.LogError("error while sending otp email for {email}", forgetPasswordDto.Email);
            return new Result(false, "error while send  otp email", null, 500);
        }

        logger.LogInformation("end generate otp for {email}", forgetPasswordDto.Email);
        return new Result(true, null, null, 204);
    }

    public async Task<Result> OtpVerification(CreateVerificationDto otp)
    {
        logger.LogInformation("start otp verification for {email}", otp.Email);

        var isExistUser = await unitOfWork.UserRepository.IsExistByEmail(otp.Email);
        if (!isExistUser)
        {
            logger.LogError("otp verification failed: user not found {email}", otp.Email);
            return new Result(false, "user not found", null, 404);
        }

        var otpResult = await unitOfWork.PasswordRepository.GetOtp(otp.Otp, otp.Email);
        if (otpResult is null)
        {
            logger.LogError("otp not found for {email}", otp.Email);
            return new Result(false, "otp not found", null, 404);
        }

        otpResult.IsValidated = true;

        unitOfWork.PasswordRepository.Update(otpResult);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("error while update otp for {email}", otp.Email);
            return new Result(false, "error while update otp", null, 500);
        }

        logger.LogInformation("end otp verification for {email}", otp.Email);
        return new Result(true, null, null, 204);
    }

    public async Task<Result> RecreatePassword(CreateRecreatePasswordDto otp)
    {
        logger.LogInformation("start recreate password for {email}", otp.Email);

        var isExistUser = await unitOfWork.UserRepository.IsExistByEmail(otp.Email);
        if (!isExistUser)
        {
            logger.LogError("recreate password failed: user not found {email}", otp.Email);
            return new Result(false, "user not found", null, 404);
        }

        var otpResult = await unitOfWork.PasswordRepository.GetOtp(otp.Otp, otp.Email, true);
        if (otpResult is null)
        {
            logger.LogError("recreate password failed: otp not found for {email}", otp.Email);
            return new Result(false, "otp not found", null, 404);
        }

        var user = await unitOfWork.UserRepository.GetUser(otp.Email);
        var validationResult = user.IsValidateFunc(false);

        if (validationResult is not null)
        {
            logger.LogError("recreate password failed for {email} validationError {status}", otp.Email, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        user?.Password = ClsUtil.HashingText(otp.Password);

        unitOfWork.UserRepository.Update(user!);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("error while update user password for {email}", otp.Email);
            return new Result(false, "error while update user password", null, 500);
        }

        var userRefreshTokenHolder = await unitOfWork.UserRefreshTokenRepository.GetByUserId(user!.Id);
        var role = userRefreshTokenHolder!.Role switch
        {
            "Admin" => EnUserType.Admin,
            "Delivery" => EnUserType.Delivery,
            _ => EnUserType.User
        };

        var tokenData = await authenticationService.GenerateToken(id: user!.Id, email: user.Email, [role]);

        logger.LogInformation("end recreate password for {email}", otp.Email);
        return new Result(true, null, tokenData, 200);
    }
}