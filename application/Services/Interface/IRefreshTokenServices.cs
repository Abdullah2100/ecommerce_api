using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services.Interface;

public interface IRefreshTokenServices
{
    Task<IActionResult> GenerateRefreshToken(ClaimsPrincipal claimsPrincipal);
}