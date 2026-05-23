using api.application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller;

[ApiController]
[Route("api/RefreshToken")]
public class RefreshTokenController(IRefreshTokenServices refreshTokenServices) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateBanner([FromQuery] string token)
    {
        var claimsPrincipal = HttpContext.User;


        return await refreshTokenServices.GenerateRefreshToken(claimsPrincipal);
    }
}