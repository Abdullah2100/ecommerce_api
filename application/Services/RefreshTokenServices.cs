using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using api.application.Interface;
using api.application.Result;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto;
using api.Presentation.dto.Response;
using api.shared.mapper;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services;

public class RefreshTokenServices(
    IUnitOfWork unitOfWork,
    IAuthenticationService authenticationService) : IRefreshTokenServices
{
    private static bool IsRefreshToken(DateTimeOffset issueAt, DateTimeOffset expireAt)
    {
        var result = issueAt - expireAt;
        return result.Days >= 29;
    }

    public async Task<IActionResult> GenerateRefreshToken(ClaimsPrincipal claimsPrincipal)
    {
        var value = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;
        var issueAt = DateTimeOffset.FromFileTime(long.Parse(claimsPrincipal.FindFirst("iat")?.Value ?? "0"));
        var expireAt = DateTimeOffset.FromFileTime(long.Parse(claimsPrincipal.FindFirst("exp")?.Value ?? "0"));

        if (value != null)
        {
            var idHolder = Guid.Parse(value);
            var user = await unitOfWork.UserRepository
                .GetUser(idHolder);

            var delivery = await unitOfWork.DeliveryRepository.GetDelivery(idHolder);

            var validationResult = user.IsValidateFunc(false);
            if (validationResult is not null && delivery is null)
            {
                return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
            }

            if (!IsRefreshToken(issueAt, expireAt))
            {
                return new ObjectResult("send valid token ")
                    { StatusCode = StatusCodes.Status400BadRequest };
            }

            var tokenHolder = authenticationService.GenerateToken(
                id: idHolder,
                email: (user?.Email ?? delivery?.User?.Email) ?? string.Empty);

            var refreshTokenHolder = authenticationService.GenerateToken(
                id: idHolder,
                email: user?.Email ?? (delivery?.User?.Email) ?? string.Empty,
                EnTokenMode.RefreshToken);


            return new ObjectResult(new AuthDto { RefreshToken = refreshTokenHolder, Token = tokenHolder })
                { StatusCode = StatusCodes.Status200OK };
        }
    }
}