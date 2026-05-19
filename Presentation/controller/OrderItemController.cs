using System.Security.Claims;
using api.application.Interface;
using api.application.Services.Interface;
using api.Filter;
using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
[Route("api/OrderItems")]
public class OrderItemController(IOrderItemServices orderItemServices) : ControllerBase
{
    [HttpGet()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrdersItemForStore
    (
        int pageNumber = 1
    )
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderItemServices
            .GetOrderItems(
                id,
                pageNumber,
                25
            );

        return result;
    }

    [HttpPut("status")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateOrderItemStatus
        ([FromBody] UpdateOrderItemStatusDto orderItemStatusDto)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderItemServices
            .UpdateOrderItemsStatus(
                id,
                orderItemStatusDto);

        return result;
    }
}