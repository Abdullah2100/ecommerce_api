using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Interface;

public interface IUserServices
{
    public Task<IActionResult> Signup(SignupDto signupDto);
    public Task<IActionResult> Login(LoginDto loginDto);


    public Task<IActionResult> GetMe(Guid id);

    public Task<IActionResult> GetUsers(int page, Guid id);
    public Task<IActionResult> GetUsersPages(Guid id, int pageLength = 25);

    public Task<IActionResult> BlockOrUnBlockUser(Guid id, Guid userId);

    public Task<IActionResult> UpdateUser(
        UpdateUserInfoDto userDto, Guid id
        , bool isUpdateWillBeUp = false);

    public Task<IActionResult> AddAddressToUser(CreateAddressDto addressDto, Guid id);
    public Task<IActionResult> UpdateUserAddress(UpdateAddressDto addressDto, Guid id);
    public Task<IActionResult> DeleteUserAddress(Guid addressId, Guid id);
    public Task<IActionResult> UpdateUserCurrentAddress(Guid addressId, Guid id);

    public Task<IActionResult> GenerateOtp(ForgetPasswordDto forgetPasswordDto);
    public Task<IActionResult> OtpVerification(CreateVerificationDto createVerificationDto);
    public Task<IActionResult> RecreatePassword(CreateRecreatePasswordDto createRecreatePasswordDto);
}