using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface IUserServices
{
    public Task<AuthDto?> Signup(SignupDto signupDto);
    public Task<AuthDto?> Login(LoginDto loginDto);


    public Task<UserInfoDto?> GetMe(Guid id);

    public Task<List<UserInfoDto>?> GetUsers(int page, Guid id);
    public Task<int?> GetUsersPages(Guid id,int pageLenght=25);

    public Task<bool> BlockOrUnBlockUser(Guid id,Guid userId);
    
    public Task<UserInfoDto?> UpdateUser(
        UpdateUserInfoDto userDto, Guid id
        ,bool isUpdateWillBeUp=false);

    public Task<AddressDto?> AddAddressToUser(CreateAddressDto addressDto, Guid id);
    public Task<AddressDto?> UpdateUserAddress(UpdateAddressDto addressDto, Guid id);
    public Task<bool> DeleteUserAddress(Guid addressId, Guid id);
    public Task<bool> UpdateUserCurrentAddress(Guid addressId, Guid id);

    public Task<bool> GenerateOtp(ForgetPasswordDto forgetPasswordDto);
    public Task<bool> OtpVerification(CreateVerificationDto createVerificationDto);
    public Task<AuthDto?> ReseatePassword(CreateRecreatePasswordDto createRecreatePasswordDto);
}