using api.application.Services.Interface;
using api.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller.v1;

[Authorize]
[ApiController]
[Route("api/subCategories")]
public class SubCategoryController(ISubCategoryServices subCategoryServices) : ControllerBase
{
    [HttpGet()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Get Sub Categories")]
    [EndpointDescription("This function is used by Admin to get Sub Categories")]

    public async Task<IActionResult> GetSubCategory(int page)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await subCategoryServices.GetSubCategoryAll(id, page, 25);

        return result;
    }
}