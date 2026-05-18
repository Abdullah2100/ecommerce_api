using api.application.Interface;
using api.application.Services.Interface;
using api.Filter;
using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
[Route("api/paymentType")]
public class PaymentTypeController(IPaymentTypeServices paymentTypeServices) : ControllerBase
{
    [HttpPost("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    public async Task<IActionResult> GetProducts([FromQuery] int pageNumber)
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var result = await paymentTypeServices.GetPaymentTypes(pageNumber);

        return result;
    }
}