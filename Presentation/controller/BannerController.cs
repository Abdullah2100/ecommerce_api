using api.application.Interface;
using api.application.Services.Interface;
using api.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
[Route("api/Banner")]
public class BannerController(IBannerServices bannerServices) : ControllerBase
{
    //this method for dashboard only
    [HttpGet()]
    [GetUserIdFromUserClaims]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBannerRandom(int pageNumber)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await bannerServices
            .GetBanners(id, pageNumber, 25);

        return result;
    }

    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBannerRandom()
    {
        var result = await bannerServices
            .GetBanners(15);

        return result;
    }
}