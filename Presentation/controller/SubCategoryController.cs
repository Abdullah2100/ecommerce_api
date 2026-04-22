using System.Security.Claims;
using api.application.Interface;
using api.Filter;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
[Route("api/subCategories")]
public class SubCategoryController(
    ISubCategoryServices subCategoryServices,
    IAuthenticationService authenticationService
) : ControllerBase
{
    [HttpGet()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetSubCategory([FromQuery()] int page)
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await subCategoryServices.GetSubCategoryAll(id, page, 25);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }
}