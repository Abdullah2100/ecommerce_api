using api.application.Interface;
using api.application.Services.Interface;
using api.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
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
    public async Task<IActionResult> GetOrderStatus()
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await analyseServices.GetMonthAnalysis(id);

        return result;
    }
}