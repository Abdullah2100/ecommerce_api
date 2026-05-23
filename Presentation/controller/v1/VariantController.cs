using api.application.Interface;
using api.Filter;
using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller;

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
    public async Task<IActionResult> DeleteVariant(Guid variantId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await variantServices.DeleteVariant(variantId, id);

        return result;
    }


    [HttpGet("{pageNumber:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVariants([FromQuery()] int pageNumber = 1)
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
    public async Task<IActionResult> GetStoresPages()
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await variantServices.GetVariantPage(id, 20);

        return result;
    }
}