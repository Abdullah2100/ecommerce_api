using api.application.Services.Interface;
using api.Filter;
using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller.v1;

[Authorize]
[ApiController]
[Route("api/Variant")]
public class VariantController(IVariantServices variantServices) : ControllerBase
{
    [HttpPost("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Create variant")]
    [EndpointDescription("This function is used by admin to create variant")]
    public async Task<IActionResult> CreateVariant([FromBody] CreateVariantDto variant)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await variantServices.CreateVariant(variant, id);

        return result;
    }

    [HttpPatch("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("update variant")]
    [EndpointDescription("This function is used by admin to update variant")]
    public async Task<IActionResult> UpdateVariant([FromBody] UpdateVariantDto variant)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await variantServices.UpdateVariant(variant, id);

        return result;
    }

    [HttpDelete("{variantId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Delete variant")]
    [EndpointDescription("This function is used by admin to delete variant")]
    public async Task<IActionResult> DeleteVariant(Guid variantId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await variantServices.DeleteVariant(variantId, id);

        return result;
    }


    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin,User")]
    [EndpointName("Get variants")]
    [EndpointDescription("This function is used by admin to create variant")]
    public async Task<IActionResult> GetVariants(int pageNumber = 1)
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");
        var result = await variantServices.GetVariants(pageNumber, 25);

        return result;
    }

    [HttpGet("pages")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Get variant pages")]
    [EndpointDescription("This function is used by admin to get variants page by page")]

    public async Task<IActionResult> GetVariantPages()
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await variantServices.GetVariantPage(id, 20);

        return result;
    }
}