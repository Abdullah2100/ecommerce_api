using api.application.Services.Interface;
using api.Filter;
using business.Services.Interface;
using data.dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
[Route("api/Category")]
public class CategoryController(ICategoryServices categoryServices,IWebHostEnvironment environment) : ControllerBase
{
    [HttpPost("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Create new category")]
    [EndpointDescription("This function is used from admin to create new category")]
    public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryDto category)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await categoryServices.CreateCategory(category, id,environment.ContentRootPath);

        return result.IsSuccessful switch
        {
            false=> new ObjectResult(result.Message){StatusCode = result.StatusCode},
            _=> new ObjectResult(result.Data){StatusCode = result.StatusCode}
        };
    }


    [HttpPut("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Update category")]
    [EndpointDescription("This function is used from admin to update category")]
    public async Task<IActionResult> UpdateCategory(
        [FromForm] UpdateCategoryDto category)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await categoryServices.UpdateCategory(category, id,environment.ContentRootPath);

        return result.IsSuccessful switch
        {
            false=> new ObjectResult(result.Message){StatusCode = result.StatusCode},
            _=> new ObjectResult(result.Data){StatusCode = result.StatusCode}
        };
    }

    [HttpDelete("{categoryId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("delete category")]
    [EndpointDescription("This function is used from admin to delete category")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await categoryServices.DeleteCategory(categoryId, id,environment.ContentRootPath);

        return result.IsSuccessful switch
        {
            false=> new ObjectResult(result.Message){StatusCode = result.StatusCode},
            _=> new ObjectResult(result.Data){StatusCode = result.StatusCode}
        };
    }


    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin, User, Store")]
    [EndpointName("Get categories by page number")]
    [EndpointDescription("This function is used to retrieve the categories by page")]
    public async Task<IActionResult> GetCategories(int pageNumber = 1)
    {
        if (pageNumber < 1)
            return BadRequest("خطء في البيانات المرسلة");

        var result = await categoryServices.GetCategories(pageNumber, 25);

        return result.IsSuccessful switch
        {
            false=> new ObjectResult(result.Message){StatusCode = result.StatusCode},
            _=> new ObjectResult(result.Data){StatusCode = result.StatusCode}
        };
    }
}