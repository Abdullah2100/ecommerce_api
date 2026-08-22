using System.Security.Claims;
using api.application;
using api.application.Services.Interface;
using api.Infrastructure;
using business.mapper;
using business.Services.Interface;
using data.dto.Response;

namespace business.Services.Implement;

public class RefreshTokenServices(
    IUnitOfWork unitOfWork,
    IAuthenticationService authenticationService) : IRefreshTokenServices
{
    private static bool IsRefreshToken(DateTimeOffset issueAt, DateTimeOffset expireAt)
    {
        var result = issueAt - expireAt;
        return result.Days >= 29;
    }

    public async Task<Result> GenerateRefreshToken(ClaimsPrincipal claimsPrincipal)
    {
        var value = claimsPrincipal.FindFirst("NameId")?.Value;
        var issueAt = DateTimeOffset.FromFileTime(long.Parse(claimsPrincipal.FindFirst("iat")?.Value ?? "0"));
        var expireAt = DateTimeOffset.FromFileTime(long.Parse(claimsPrincipal.FindFirst("exp")?.Value ?? "0"));

        if (value == null)
            return new Result(false, "error Credentials", null,401);

        var idHolder = Guid.Parse(value);

        var user = await unitOfWork.UserRepository.GetUser(idHolder);
        var delivery = await unitOfWork.DeliveryRepository.GetDelivery(idHolder);

        var validationResult = user.IsValidateFunc(false);

        if (validationResult is not null && delivery is null)
        {
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        if (!IsRefreshToken(issueAt, expireAt))
        {
            return new Result(false, "send valid token ", null, 400);
        }

        var userRefreshTokenHolder = await unitOfWork.UserRefreshTokenRepository.GetByUserId(user!.Id);

        if (userRefreshTokenHolder is null)
            return new Result(false, "error Credentials", null, 401);

        var roleList = new List<EnUserType> { user.IsUser ? EnUserType.User : EnUserType.Admin };

        if (user.Store != null)
            roleList.Add(EnUserType.Store);

        if (delivery != null)
            roleList.Add(EnUserType.Delivery);

        var tokenData = await authenticationService.GenerateToken(
            id: user!.Id,
            email: user.Email,
            roleList
        );

        return new Result(true, null, tokenData, 200);
    }
}