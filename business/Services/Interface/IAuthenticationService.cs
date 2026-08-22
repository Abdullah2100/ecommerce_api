using business.Services.Implement;
using data.dto.Response;

namespace business.Services.Interface;

public interface IAuthenticationService
{
    Task<AuthDto> GenerateToken(Guid id, string email, List<EnUserType> types);
}