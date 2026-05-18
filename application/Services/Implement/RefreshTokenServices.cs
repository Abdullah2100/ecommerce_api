using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using api.application.Interface;
using api.application.Result;
using api.application.Services.Implement;
using api.application.Services.Interface;
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

        if (value == null)
            return new ObjectResult("error Credentials") { StatusCode = StatusCodes.Status401Unauthorized };

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

        
        var userRefreshTokenHolder = await unitOfWork.UserRefreshTokenRepository.GetByUserId(user!.Id);

        var role = userRefreshTokenHolder!.Role switch
        {
            "Admin" => EnUserType.Admin,
            "Delivery" => EnUserType.Delivery,
            _ => EnUserType.User
        };


        var tokenData = await authenticationService.GenerateToken(
            id: user!.Id,
            email: user.Email,
            role
        );


        return new ObjectResult(tokenData)
            { StatusCode = StatusCodes.Status200OK };
    }
}