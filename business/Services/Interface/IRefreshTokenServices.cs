using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using api.application;

namespace api.application.Services.Interface;

public interface IRefreshTokenServices
{
    Task<Result> GenerateRefreshToken(ClaimsPrincipal claimsPrincipal);
}