using api.application.Services.Interface;
using api.Filter;
using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller.v1;

[Authorize]
[ApiController]
[Route("api/Currencies")]
public class CurrencyController(ICurrencyServices currencyServices) : ControllerBase
{
    [HttpPost("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Create new Currency")]
    [EndpointDescription("This function is used from admin to create currency")]

    public async Task<IActionResult> CreateCurrency([FromBody] CreateCurrencyDto currencyDto)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await currencyServices.CreateCurrency(id, currencyDto);

        return result;
    }

    [HttpPut("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("update Currency")]
    [EndpointDescription("This function is used from admin to update currency")]

    public async Task<IActionResult> UpdateCurrency([FromBody] UpdateCurrencyDto currencyDto)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await currencyServices.UpdateCurrency(id, currencyDto);

        return result;
    }

    [HttpDelete("{currencyId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Delete Currency")]
    [EndpointDescription("This function is used from admin to delete currency")]

    public async Task<IActionResult> DeleteCurrency(Guid currencyId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await currencyServices.DeleteCurrency(id, currencyId);

        return result;
    }


    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin, User, Store")]
    [EndpointName($"Get currencies")]
    [EndpointDescription("This function is used to  retrieve the currencies")]

    public async Task<IActionResult> GetCurrencies(int pageNumber = 1)
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");
        var result = await currencyServices.GetCurrency(pageNumber, 25);

        return result;
    }
}