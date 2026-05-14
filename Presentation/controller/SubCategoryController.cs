using api.application.Interface;
using api.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
[Route("api/subCategories")]
public class SubCategoryController(ISubCategoryServices subCategoryServices) : ControllerBase
{
    [HttpGet()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetSubCategory(int page)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await subCategoryServices.GetSubCategoryAll(id, page, 25);

        return result;
    }
}