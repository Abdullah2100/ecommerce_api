using api.application.Services.Implement;
using api.Presentation.dto.Response;

namespace api.application.Services.Interface;

public interface IAuthenticationService
{
     Task<AuthDto> GenerateToken(Guid id, string email, List<EnUserType> types);

}