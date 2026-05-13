using api.application.Interface;
using api.application.Result;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services;

public class UserService(
    IConfig config,
    IFileServices fileServices,
    IUnitOfWork unitOfWork,
    IAuthenticationService authenticationService,
    IServiceProvider sp
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

        string token = "", refreshToken = "";

        token = authenticationService.GenerateToken(
            id: userId,
            email: signupDto.Email);

        refreshToken = authenticationService.GenerateToken(
            id: userId,
            email: signupDto.Email,
            EnTokenMode.RefreshToken);


        return new ObjectResult(new AuthDto { RefreshToken = refreshToken, Token = token })
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


        string token = "", refreshToken = "";

        token = authenticationService.GenerateToken(
            id: user.Id,
            email: user.Email);

        refreshToken = authenticationService.GenerateToken(
            id: user.Id,
            email: user.Email,
            EnTokenMode.RefreshToken);

        return new ObjectResult(new AuthDto { RefreshToken = refreshToken, Token = token })
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

        var userToDto = user!.ToUserInfoDto(config.GetKey("url_file"));
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

        var usersToDto = (await unitOfWork.UserRepository
                .GetUsers(page, 25))
            .Select(u => u.ToUserInfoDto(config.GetKey("url_file")))
            .ToList();


        return new ObjectResult(usersToDto)
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

        //var userToDto = user?.ToUserInfoDto(config.GetKey("url_file"));

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

        int addressCount = await unitOfWork.AddressRepository.GetAddressCount(id);

        if (addressCount == 20)
        {
            return new Result<AddressDto?>
            (
                data: null,
                message: "maximum 20 addresses reached",
                isSuccessful: false,
                statusCode: 400
            );
        }

        Address address = new Address
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
            return new Result<AddressDto?>
            (
                data: null,
                message: "error while adding address",
                isSuccessful: false,
                statusCode: 400
            );
        }

        return new Result<AddressDto?>
        (
            data: address.ToDto(),
            message: "",
            isSuccessful: true,
            statusCode: 201
        );
    }


    public async Task<AddressDto?>> UpdateUserAddress(
        UpdateAddressDto addressDto,
        Guid id)
    {
        if (addressDto.IsEmpty())
            return new Result<AddressDto?>
            (
                data: null,
                message: "nothing to be updated",
                isSuccessful: true,
                statusCode: 200
            );

        User? user = await unitOfWork.UserRepository
            .GetUser(id);
        var isValide = user.IsValidateFunc(false);

        if (isValide is not null)
        {
            return new Result<AddressDto?>(
                isSuccessful: false,
                data: null,
                message: isValide.Message,
                statusCode: isValide.StatusCode
            );
        }

        if (
            (addressDto.Longitude is null && addressDto.Latitude is not null) ||
            (addressDto.Longitude is not null && addressDto.Latitude is null)
        )
        {
            return new Result<AddressDto?>(
                isSuccessful: false,
                data: null,
                message: "when update address you must change both longitude and latitude not one of them only ",
                statusCode: 400
            );
        }


        Address? address = await unitOfWork.AddressRepository.GetAddress(addressDto.Id);

        if (address is null)
        {
            return new Result<AddressDto?>
            (
                data: null,
                message: "address not found",
                isSuccessful: false,
                statusCode: 404
            );
        }

        if (address.OwnerId != id)
        {
            return new Result<AddressDto?>
            (
                data: null,
                message: "address not owned",
                isSuccessful: false,
                statusCode: 400
            );
        }


        address.Longitude = addressDto.Longitude ?? address.Longitude;
        address.Title = addressDto.Title ?? address.Title;
        address.Latitude = addressDto.Latitude ?? address.Latitude;

        unitOfWork.AddressRepository.Update(address);
        int result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new Result<AddressDto?>
            (
                data: null,
                message: "error while updating address",
                isSuccessful: false,
                statusCode: 400
            );
        }

        return new Result<AddressDto?>
        (
            data: address.ToDto(),
            message: "",
            isSuccessful: true,
            statusCode: 200
        );
    }


    public async Task<bool>> DeleteUserAddress(Guid addressId, Guid id)
    {
        User? user = await unitOfWork.UserRepository
            .GetUser(id);
        var isValide = user.IsValidateFunc(false);

        if (isValide is not null)
        {
            return new Result<bool>(
                isSuccessful: false,
                data: false,
                message: isValide.Message,
                statusCode: isValide.StatusCode
            );
        }

        Address? address = await unitOfWork.AddressRepository.GetAddress(addressId);

        if (address is null)
        {
            return new Result<bool>
            (
                data: false,
                message: "address not found",
                isSuccessful: false,
                statusCode: 404
            );
        }

        if (address.OwnerId != id)
        {
            return new Result<bool>
            (
                data: false,
                message: "address not owned",
                isSuccessful: false,
                statusCode: 400
            );
        }

        if (address.IsCurrent)
        {
            return new Result<bool>
            (
                data: false,
                message: "could not delete current address",
                isSuccessful: false,
                statusCode: 400
            );
        }

        unitOfWork.AddressRepository.Delete(addressId);
        int result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new Result<bool>
            (
                data: false,
                message: "error while delete address",
                isSuccessful: false,
                statusCode: 400
            );
        }

        return new Result<bool>
        (
            data: true,
            message: "",
            isSuccessful: true,
            statusCode: 204
        );
    }


    public async Task<bool>> UpdateUserCurrentAddress(Guid addressId, Guid id)
    {
        User? user = await unitOfWork.UserRepository
            .GetUser(id);
        var isValide = user.IsValidateFunc(false);

        if (isValide is not null)
        {
            return new Result<bool>(
                isSuccessful: false,
                data: false,
                message: isValide.Message,
                statusCode: isValide.StatusCode
            );
        }

        Address? address = await unitOfWork.AddressRepository.GetAddress(addressId);

        if (address is null)
        {
            return new Result<bool>
            (
                data: false,
                message: "address not found",
                isSuccessful: false,
                statusCode: 404
            );
        }

        if (address.OwnerId != id)
        {
            return new Result<bool>
            (
                data: false,
                message: "address not owned",
                isSuccessful: false,
                statusCode: 400
            );
        }

        if (address.IsCurrent)
        {
            return new Result<bool>
            (
                data: false,
                message: "address is already current address",
                isSuccessful: false,
                statusCode: 400
            );
        }

        unitOfWork.AddressRepository.MakeAddressNotCurrentToId(user!.Id);


        unitOfWork.AddressRepository.UpdateCurrentLocation(addressId, user!.Id);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new Result<bool>
            (
                data: false,
                message: "error while update current address",
                isSuccessful: false,
                statusCode: 400
            );
        }

        return new Result<bool>
        (
            data: true,
            message: "",
            isSuccessful: true,
            statusCode: 204
        );
    }


    public async Task<bool>> GenerateOtp(ForgetPasswordDto forgetPasswordDto)
    {
        User? user = await unitOfWork.UserRepository
            .GetUser(forgetPasswordDto.Email);

        var isValide = user.IsValidateFunc(false);

        if (isValide is not null)
        {
            return new Result<bool>(
                isSuccessful: false,
                data: false,
                message: isValide.Message,
                statusCode: isValide.StatusCode
            );
        }

        string otp = ClsUtil.GenerateGuid().ToString().Substring(0, 6).Replace("-", "");
        bool isOtpExist = await unitOfWork.PasswordRepository.IsExist(otp, user!.Email);
        bool isExist = isOtpExist;

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
        int result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new Result<bool>
            (
                data: false,
                message: "error while generate otp",
                isSuccessful: false,
                statusCode: 400
            );
        }


        var sendMessageService = sp.GetRequiredKeyedService<IMessageService>(EnMessageService.Email);
        bool emailSendResult = await sendMessageService.SendingMessage(message: otp, otp);

        if (!emailSendResult)
        {
            return new Result<bool>
            (
                data: false,
                message: "error while send  otp email",
                isSuccessful: false,
                statusCode: 400
            );
        }

        return new Result<bool>
        (
            data: true,
            message: "",
            isSuccessful: false,
            statusCode: 204
        );
    }

    public async Task<bool>> OtpVerification(CreateVerificationDto otp)
    {
        bool isExistUser = await unitOfWork.UserRepository
            .IsExistByEmail(otp.Email);
        if (!isExistUser)
        {
            return new Result<bool>
            (
                data: false,
                message: "user not found",
                isSuccessful: false,
                statusCode: 404
            );
        }

        ReseatPasswordOtp? otpResult = await unitOfWork.PasswordRepository.GetOtp(otp.Otp, otp.Email);


        if (otpResult is null)
        {
            return new Result<bool>
            (
                data: false,
                message: "otp not found",
                isSuccessful: false,
                statusCode: 404
            );
        }

        otpResult.IsValidated = true;

        unitOfWork.PasswordRepository.Update(otpResult);
        int result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new Result<bool>
            (
                data: false,
                message: "error while update otp",
                isSuccessful: false,
                statusCode: 400
            );
        }


        return new Result<bool>
        (
            data: true,
            message: "",
            isSuccessful: true,
            statusCode: 204
        );
    }

    public async Task<AuthDto?>> RecreatePassword(CreateRecreatePasswordDto otp)
    {
        bool isExistUser = await unitOfWork.UserRepository
            .IsExistByEmail(otp.Email);
        if (!isExistUser)
        {
            return new Result<AuthDto?>
            (
                data: null,
                message: "user not found",
                isSuccessful: false,
                statusCode: 404
            );
        }

        ReseatPasswordOtp? otpResult = await unitOfWork.PasswordRepository.GetOtp(otp.Otp, otp.Email, true);


        if (otpResult is null)
        {
            return new Result<AuthDto?>
            (
                data: null,
                message: "otp not found",
                isSuccessful: false,
                statusCode: 404
            );
        }

        User? user = await unitOfWork.UserRepository.GetUser(otp.Email);

        var isValide = user.IsValidateFunc();
        if (isValide is not null)
        {
            return new Result<AuthDto?>(
                isSuccessful: false,
                data: null,
                message: isValide.Message,
                statusCode: isValide.StatusCode
            );
        }

        user.Password = ClsUtil.HashingText(otp.Password);

        unitOfWork.UserRepository.Update(user);
        int result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new Result<AuthDto?>
            (
                data: null,
                message: "error while update user password",
                isSuccessful: false,
                statusCode: 400
            );
        }


        string token = "", refreshToken = "";

        token = authenticationService.GenerateToken(
            id: user.Id,
            email: user.Email
        );

        refreshToken = authenticationService.GenerateToken(
            id: user.Id,
            email: user.Email,
            EnTokenMode.RefreshToken);

        return new Result<AuthDto?>(
            isSuccessful: true,
            data: new AuthDto { RefreshToken = refreshToken, Token = token },
            message: "",
            statusCode: 200
        );
    }
}