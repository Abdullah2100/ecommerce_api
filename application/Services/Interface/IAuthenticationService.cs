using System.Security.Claims;
using api.application.Services;

namespace api.application.Interface;

public interface IAuthenticationService
{
    string GenerateToken(Guid id, string email, EnTokenMode tokenType = EnTokenMode.AccessToken);
}