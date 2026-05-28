using api.application.Services.Interface;
using api.Filter;
using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller.v1;

[Authorize]
[ApiController]
[Route("api/paymentType")]
public class PaymentTypeController(IPaymentTypeServices paymentTypeServices) : ControllerBase
{
    [HttpPost("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("create paymentType")]
    [EndpointDescription("This function is used by admin to create new paymentType")]

    public async Task<IActionResult> CreatePaymentType
    (
        [FromForm] CreatePaymentTypeDto paymentTypeDto
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await paymentTypeServices.Create(paymentTypeDto, id);

        return result;
    }


    [HttpPut("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("update paymentType")]
    [EndpointDescription("This function is used by admin to update paymentType")]

    public async Task<IActionResult> UpdatePaymentType
    (
        [FromForm] UpdatePaymentTypeDto paymentTypeDto
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await paymentTypeServices.Update(paymentTypeDto, id);

        return result;
    }

    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Get paymentTypes")]
    [EndpointDescription("This function is used by admin to get paymentTypes list by page")]

    public async Task<IActionResult> GetPaymentTypes([FromQuery] int pageNumber)
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var result = await paymentTypeServices.GetPaymentTypes(pageNumber);

        return result;
    }
}