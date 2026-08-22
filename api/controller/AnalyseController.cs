using api.Filter;
using Asp.Versioning;
using business.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/analyse")]
public class AnalyseController(IAnalyseServices analyseServices) : ControllerBase
{
    [HttpGet("system")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Current month Analysis")]
    [EndpointDescription(
        "This function is returning the Analysis of the api like total fee ,total order and total delivery distance  for current month for Admin")]
    public async Task<IActionResult> GetOrderStatus()
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await analyseServices.GetMonthAnalysis(id);

        return result.IsSuccessful switch
        {
            false=> new ObjectResult(result.Message){StatusCode = result.StatusCode},
            _=> new ObjectResult(result.Data){StatusCode = result.StatusCode}
        };
    }
}