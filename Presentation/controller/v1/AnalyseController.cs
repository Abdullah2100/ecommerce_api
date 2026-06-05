using api.application.Services.Interface;
using api.Filter;
using api.Presentation.dto.Request;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace api.Presentation.controller.v1;

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

        return result;
    }
}