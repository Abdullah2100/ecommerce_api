using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;

namespace api.application.Services.Implement;

public class UserService(
    IConfiguration config,
    IFileServices fileServices,
    IUnitOfWork unitOfWork,
    IAuthenticationService authenticationService,
    IServiceProvider sp,
    HybridCache cache
)
    : IUserServices
{
    public async Task<IActionResult> Signup(SignupDto signupDto)
    {
        var validationResult = ClsValidation
            .ValidateInput(
                signupDto.Email,
                signupDto.Password,
                signupDto.Phone
            );

        if (validationResult != null)
        {
            return new ObjectResult(validationResult)
                { StatusCode = StatusCodes.Status400BadRequest };
        }

        var isExistByEmail = await unitOfWork.UserRepository.IsExistByEmail(signupDto.Email);

        if (isExistByEmail)
        {
            return new ObjectResult("email already exist")
                { StatusCode = StatusCodes.Status409Conflict };
        }

        var isExistByPhone = (await unitOfWork.UserRepository.IsExistByPhone(signupDto.Phone));

        if (isExistByPhone)
        {
            return new ObjectResult("phone already exist")
                { StatusCode = StatusCodes.Status409Conflict };
        }

        if (signupDto.Role == 0 && await unitOfWork.UserRepository.IsExist(false))
        {
            return new ObjectResult("you cannot create a user with exist role")
                { StatusCode = StatusCodes.Status403Forbidden };
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
            return new ObjectResult("there are error in create new user")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }


        var tokenData = await authenticationService.GenerateToken(
            id: userId,
            email: signupDto.Email,
            [EnUserType.User]);


        return new ObjectResult(tokenData)
            { StatusCode = StatusCodes.Status200OK };
    }

    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(loginDto.Username,
                ClsUtil.HashingText(loginDto.Password)
            );

        var validationResult = user.IsValidateFunc(false);
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        user!.DeviceToken = loginDto.DeviceToken;
        unitOfWork.UserRepository.Update(user);

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
            return new ObjectResult("Error While Login User")
                { StatusCode = StatusCodes.Status500InternalServerError };


        var userRefreshTokenHolder = await unitOfWork.UserRefreshTokenRepository.GetByUserId(user!.Id);

        var role = userRefreshTokenHolder!.Role switch
        {
            "Admin" => EnUserType.Admin,
            "Delivery" => EnUserType.Delivery,
            _ => EnUserType.User
        };


        var tokenData = await authenticationService.GenerateToken(
            id: user!.Id,
            email: user.Email,
            [role]
        );


        return new ObjectResult(tokenData)
            { StatusCode = StatusCodes.Status200OK };
    }


    public async Task<IActionResult> GetMe(Guid id)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(id);

        var validationResult = user.IsValidateFunc(false);
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var userToDto = user!.ToUserInfoDto(config["url_file"] ?? "");
        return new ObjectResult(userToDto) { StatusCode = StatusCodes.Status200OK };
    }


    public async Task<IActionResult> GetUsers(
        int page,
        Guid id)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(id);

        var validationResult = user.IsValidateFunc();
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var users = await cache.GetOrCreateAsync(MemoryCacheKeys.UsersKey + "/" + id + "/" + page,
            async ct =>
            {
                var users = (await unitOfWork.UserRepository
                        .GetUsers(page, 25))
                    .Select(u => u.ToUserInfoDto(config["url_file"] ?? ""))
                    .ToList();
                return users;
            },
            tags: [MemoryCacheKeys.UsersKey]);


        return new ObjectResult(users)
            { StatusCode = StatusCodes.Status200OK };
    }

    public async Task<IActionResult> GetUsersPages(Guid id, int pageLenght)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(id);

        var validationResult = user.IsValidateFunc();
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var userPages = await unitOfWork.UserRepository.GetUserCount();
        var pageUserCount = userPages > 0 ? (int)Math.Ceiling((double)userPages / pageLenght) : 0;

        return new ObjectResult(pageUserCount)
            { StatusCode = StatusCodes.Status200OK };
    }

    public async Task<IActionResult> BlockOrUnBlockUser(Guid id, Guid userId)
    {
        var admin = await unitOfWork.UserRepository
            .GetUser(id);

        var validationResult = admin.IsValidateFunc();
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var user = await unitOfWork.UserRepository.GetUser(userId);

        validationResult = user.IsValidateFunc();

        //this to handle if user that admin want to block is not admin
        if (validationResult is not null)
        {
            return new ObjectResult($"unable to {(user?.IsBlocked == true ? "block" : "unblock")}  user")
                { StatusCode = StatusCodes.Status403Forbidden };
        }

        user!.IsBlocked = !user.IsBlocked;

        if (user is { IsBlocked: true, IsUser: false })
        {
            return new ObjectResult("you could not block admin user ")
                { StatusCode = StatusCodes.Status403Forbidden };
        }

        unitOfWork.UserRepository.Update(user);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while change user Blocking status")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.UsersKey);


        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }


    public async Task<IActionResult> UpdateUser(
        UpdateUserInfoDto userDto,
        Guid id,
        bool isUpdateWillBeTop = false)
    {
        if (userDto.IsEmpty())
            return new ObjectResult("no data changes")
                { StatusCode = StatusCodes.Status400BadRequest };


        var user = await unitOfWork.UserRepository.GetUser(id);

        var validationResult = user.IsValidateFunc(false);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }


        if (userDto.Phone is not null && user?.Phone != userDto.Phone)
        {
            var isExistPhone = await unitOfWork.UserRepository.IsExistByPhone(userDto.Phone ?? "");

            if (isExistPhone)
            {
                return new ObjectResult("phone already exist")
                {
                    StatusCode = StatusCodes.Status409Conflict
                };
            }
        }

        var hashedPassword =
            string.IsNullOrEmpty(userDto.Password)
            || string.IsNullOrEmpty(userDto.NewPassword)
                ? null
                : ClsUtil.HashingText(userDto.NewPassword);

        if (userDto is { Password: not null, NewPassword: not null })
        {
            if (user?.Password != ClsUtil.HashingText(userDto.Password))
            {
                return new ObjectResult("Enter Valid Previous Password")
                {
                    StatusCode = StatusCodes.Status409Conflict
                };
            }
        }

        string? profile = null;
        if (userDto.Thumbnail != null)
        {
            profile = await fileServices.SaveFile(userDto.Thumbnail, EnImageType.Profile);
        }

        user?.Thumbnail = profile ?? user.Thumbnail;
        user?.Name = userDto.Name ?? user.Name;
        user?.Phone = userDto.Phone ?? user.Phone;
        user?.UpdatedAt = DateTime.Now;
        user?.Password = hashedPassword ?? user.Password;

        unitOfWork.UserRepository.Update(user!);

        if (isUpdateWillBeTop)
        {
            return new ObjectResult(null)
                { StatusCode = StatusCodes.Status204NoContent };
        }

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while updating user")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        //var userToDto = user?.ToUserInfoDto(config["url_file"]??"");

        await cache.RemoveByTagAsync(MemoryCacheKeys.UsersKey);

        
        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> AddAddressToUser(
        CreateAddressDto addressDto,
        Guid id
    )
    {
        var user = await unitOfWork.UserRepository
            .GetUser(id);

        var validationResult = user.IsValidateFunc(false);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var addressCount = await unitOfWork.AddressRepository.GetAddressCount(id);

        if (addressCount == 20)
        {
            return new ObjectResult("maximum 20 addresses reached") { StatusCode = StatusCodes.Status403Forbidden };
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
            return new ObjectResult("error while adding address")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.UsersKey);

        return new ObjectResult(address.ToDto()) { StatusCode = StatusCodes.Status201Created };
    }


    public async Task<IActionResult> UpdateUserAddress(
        UpdateAddressDto addressDto,
        Guid id)
    {
        if (addressDto.IsEmpty())
            return new ObjectResult("nothing to be updated")
                { StatusCode = StatusCodes.Status400BadRequest };


        var user = await unitOfWork.UserRepository
            .GetUser(id);

        var validationResult = user.IsValidateFunc(false);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }


        if (
            (addressDto.Longitude is null && addressDto.Latitude is not null) ||
            (addressDto.Longitude is not null && addressDto.Latitude is null)
        )
        {
            return new ObjectResult(
                    "when update address you must change both longitude and latitude not one of them only ")
                { StatusCode = StatusCodes.Status400BadRequest };
        }


        var address = await unitOfWork.AddressRepository.GetAddress(addressDto.Id);

        if (address is null)
        {
            return new ObjectResult("address not found")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        if (address.OwnerId != id)
        {
            return new ObjectResult("address not belong to you")
                { StatusCode = StatusCodes.Status404NotFound };
        }


        address.Longitude = addressDto.Longitude ?? address.Longitude;
        address.Title = addressDto.Title ?? address.Title;
        address.Latitude = addressDto.Latitude ?? address.Latitude;

        unitOfWork.AddressRepository.Update(address);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while updating address")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }
        
        await cache.RemoveByTagAsync(MemoryCacheKeys.UsersKey);


        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }


    public async Task<IActionResult> DeleteUserAddress(Guid addressId, Guid id)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(id);

        var validationResult = user.IsValidateFunc(false, isStore: true);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }


        var address = await unitOfWork.AddressRepository.GetAddress(addressId);

        if (address is null)
        {
            return new ObjectResult("address not found")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        if (address.OwnerId != id)
        {
            return new ObjectResult("address not belong to you")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        if (address.IsCurrent)
        {
            return new ObjectResult("could not delete current address")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        unitOfWork.AddressRepository.Delete(addressId);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while delete address")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.UsersKey);

        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }


    public async Task<IActionResult> UpdateUserCurrentAddress(Guid addressId, Guid id)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(id);

        var validationResult = user.IsValidateFunc(false);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var address = await unitOfWork.AddressRepository.GetAddress(addressId);

        if (address is null)
        {
            return new ObjectResult("address not found")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        if (address.OwnerId != id)
        {
            return new ObjectResult("address not belong to you")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        if (address.IsCurrent)
        {
            return new ObjectResult("address is already current address")
                { StatusCode = StatusCodes.Status409Conflict };
        }

        unitOfWork.AddressRepository.MakeAddressNotCurrentToId(user!.Id);


        unitOfWork.AddressRepository.UpdateCurrentLocation(addressId, user!.Id);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while update current address")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.UsersKey);
        
        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }


    public async Task<IActionResult> GenerateOtp(ForgetPasswordDto forgetPasswordDto)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(forgetPasswordDto.Email);

        var validationResult = user.IsValidateFunc(false);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
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

        unitOfWork.PasswordRepository.Add(
            new ReseatPasswordOtp
            {
                Email = forgetPasswordDto.Email,
                CreatedAt = DateTime.Now.AddHours(1),
                Id = ClsUtil.GenerateGuid(),
                Otp = otp
            }
        );

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while generate otp")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }


        var sendMessageService = sp.GetRequiredKeyedService<IMessageService>(EnMessageService.Email);
        var emailSendResult = await sendMessageService.SendingMessage(message: otp, otp);

        if (!emailSendResult)
        {
            return new ObjectResult("error while send  otp email")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> OtpVerification(CreateVerificationDto otp)
    {
        var isExistUser = await unitOfWork.UserRepository
            .IsExistByEmail(otp.Email);
        if (!isExistUser)
        {
            return new ObjectResult("user not found")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        var otpResult = await unitOfWork.PasswordRepository.GetOtp(otp.Otp, otp.Email);


        if (otpResult is null)
        {
            return new ObjectResult("otp not found")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        otpResult.IsValidated = true;

        unitOfWork.PasswordRepository.Update(otpResult);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while update otp")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }


        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> RecreatePassword(CreateRecreatePasswordDto otp)
    {
        var isExistUser = await unitOfWork.UserRepository
            .IsExistByEmail(otp.Email);

        if (!isExistUser)
        {
            return new ObjectResult("user not found")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        var otpResult = await unitOfWork.PasswordRepository.GetOtp(otp.Otp, otp.Email, true);


        if (otpResult is null)
        {
            return new ObjectResult("otp not found")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        var user = await unitOfWork.UserRepository.GetUser(otp.Email);

        var validationResult = user.IsValidateFunc(false);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        user?.Password = ClsUtil.HashingText(otp.Password);

        unitOfWork.UserRepository.Update(user!);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while update user password")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var userRefreshTokenHolder = await unitOfWork.UserRefreshTokenRepository.GetByUserId(user!.Id);

        var role = userRefreshTokenHolder!.Role switch
        {
            "Admin" => EnUserType.Admin,
            "Delivery" => EnUserType.Delivery,
            _ => EnUserType.User
        };


        var tokenData = await authenticationService.GenerateToken(
            id: user!.Id,
            email: user.Email,
            [role]
        );


        return new ObjectResult(tokenData)
            { StatusCode = StatusCodes.Status200OK };
    }
}