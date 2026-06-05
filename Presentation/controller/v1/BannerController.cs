using api.application.Services.Interface;
using api.Filter;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller.v1;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/Banner")]
public class BannerController(IBannerServices bannerServices) : ControllerBase
{
    //this method for dashboard only
    [HttpGet()]
    [GetUserIdFromUserClaims]
    [Authorize(Roles = "Store, User ,Admin")]
    [EndpointName("Get banners belong to user")]
    [EndpointDescription(
        "This function is returning the Analysis of the api like total fee ,total order and total delivery distance  for current month for Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBannerRandom(int pageNumber)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await bannerServices
            .GetBanners(id, pageNumber, 25);

        return result;
    }

    [HttpGet("")]
    [Authorize(Roles = "Store, User ,Admin")]
    [EndpointName("Get random Banners")]
    [EndpointDescription(
        "This function is returning random banner belong to many store it used in ecommerce app for user")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBannerRandom()
    {
        var result = await bannerServices
            .GetBanners(15);

        return result;
    }
}