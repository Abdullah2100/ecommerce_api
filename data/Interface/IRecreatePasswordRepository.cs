using api.domain.entity;

namespace data.Interface;

public interface IRecreatePasswordRepository : IRepository<ReseatPasswordOtp>
{
    Task<bool> IsExist(string otp, string email);
    Task<ReseatPasswordOtp?> GetOtp(string otp, string email, bool state = false);
    Task<ReseatPasswordOtp?> GetOtp(string otp);
    Task Delete(Guid id);
}