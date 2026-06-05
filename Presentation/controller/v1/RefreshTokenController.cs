using api.application.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller.v1;

[ApiController]
[Route("api/RefreshToken")]
public class RefreshTokenController(IRefreshTokenServices refreshTokenServices) : ControllerBase
{
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Store,Admin")]
    [EndpointName("create Banner")]
    [EndpointDescription("This function is used by Store or Admin to create Banner")]
    public async Task<IActionResult> CreateBanner([FromQuery] string token)
    {
        var claimsPrincipal = HttpContext.User;


        return await refreshTokenServices.GenerateRefreshToken(claimsPrincipal);
    }
}