using api.application.Interface;
using api.application.Services.Interface;
using api.Filter;
using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
[Route("api/Order")]
public class OrderController(IOrderServices orderServices) : ControllerBase
{
    [HttpPost()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateOrder
        ([FromBody] CreateOrderDto orderDto)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices.CreateOrder(id, orderDto);

        return result;
    }

    [HttpGet("status")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetOrderStatus()
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices.GetOrdersStatus(id);

        return result;
    }

    [HttpGet()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetOrders
    (
        int pageNumber = 1
    )
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices.GetOrders(id, pageNumber, 25);

        return result;
    }


    [HttpGet("me")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyOrders
    (
        int pageNumber = 1
    )
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices
            .GetMyOrders(
                id,
                pageNumber,
                25);

        return result;
    }

    [HttpGet("deliveries")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrderNotBelongToDelivery
        (int pageNumber = 1)
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices
            .GetOrdersNotBelongToDeliveries(
                id,
                pageNumber,
                25);

        return result;
    }


    [HttpDelete("{orderId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteOrders
        (Guid orderId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await orderServices
            .DeleteOrder(
                orderId, id);

        return result;
    }


    [HttpPut()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateOrderStatus
    (
        [FromBody] UpdateOrderStatusDto orderStatus
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices
            .UpdateOrderStatus(
                orderStatus.Id,
                orderStatus.Status
            );

        return result;
    }
}