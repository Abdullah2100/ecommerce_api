using System.Security.Claims;
using api.application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller;

[ApiController]
[Route("api/RefreshToken")]
public class RefreshTokenController(
    IAuthenticationService authenticationService,
    IRefreshTokenServices refreshTokenServices): ControllerBase
{
    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateBanner([FromQuery]string token)
    {
        Claim? id = User.Claims.First(value=>value.Type== ClaimTypes.NameIdentifier);
        Claim? issueAt = User.FindFirst("exp");
        Claim? expire =User.FindFirst("lat");
       
        var result = await refreshTokenServices.GenerateRefreshToken(token, id, issueAt, expire);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


}