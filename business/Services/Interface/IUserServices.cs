using api.application;
using data.dto.Request;
using data.dto.Response;

namespace business.Services.Interface;

public interface IUserServices
{
    public Task<Result> Signup(SignupDto signupDto);
    public Task<Result> Login(LoginDto loginDto);


    public Task<Result> GetMe(Guid id);

    public Task<Result> GetUsers(int page, Guid id);
    public Task<Result> GetUsersPages(Guid id, int pageLength = 25);

    public Task<Result> BlockOrUnBlockUser(Guid id, Guid userId);

    public Task<Result> UpdateUser(
        UpdateUserInfoDto userDto, Guid id,string rootPath
        , bool isUpdateWillBeUp = false);

    public Task<Result> AddAddressToUser(CreateAddressDto addressDto, Guid id);
    public Task<Result> UpdateUserAddress(UpdateAddressDto addressDto, Guid id);
    public Task<Result> DeleteUserAddress(Guid addressId, Guid id);
    public Task<Result> UpdateUserCurrentAddress(Guid addressId, Guid id);

    public Task<Result> GenerateOtp(ForgetPasswordDto forgetPasswordDto);
    public Task<Result> OtpVerification(CreateVerificationDto createVerificationDto);
    public Task<Result> RecreatePassword(CreateRecreatePasswordDto createRecreatePasswordDto);
}