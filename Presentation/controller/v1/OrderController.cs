using api.application.Services.Interface;
using api.Filter;
using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller.v1;

[Authorize]
[ApiController]
[Route("api/Order")]
public class OrderController(IOrderServices orderServices) : ControllerBase
{
    [HttpPost()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin,User")]
    [EndpointName("Submit order")]
    [EndpointDescription("This function is used to submit order from user or admin from dashboard ")]
    public async Task<IActionResult> CreateOrder
        ([FromBody] CreateOrderDto orderDto)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices.CreateOrder(id, orderDto);

        return result;
    }

    [HttpGet("orderStatusList")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Get order Status List")]
    [EndpointDescription("This function is used by admin at dashboard to convert the order enum to readable string that admin can understand it ")]
    public async Task<IActionResult> GetOrderStatus()
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices.GetOrdersStatus(id);

        return result;
    }

    [HttpGet()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Get orders")]
    [EndpointDescription("This function is used by admin at dashboard to get order page by page")]
    public async Task<IActionResult> GetOrders(int pageNumber = 1)
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices.GetOrders(id, pageNumber, 25);

        return result;
    }


    [HttpGet("me")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "User")]
    [EndpointName("Get Current User Order")]
    [EndpointDescription("This function is used by user to his own orders by pages")]
    public async Task<IActionResult> GetMyOrders(int pageNumber = 1)
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Delivery")]
    [EndpointName("Get orders not belong to any deliveries")]
    [EndpointDescription("This function is used by deliveries to orders not belong to another deliveries  by pages")]

    public async Task<IActionResult> GetOrderNotBelongToDelivery(int pageNumber = 1)
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin,User")]
    [EndpointName("Delete order ")]
    [EndpointDescription("This function is used by admin or User to delete there order by id")]
    public async Task<IActionResult> DeleteOrders(Guid orderId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await orderServices
            .DeleteOrder(
                orderId, id);

        return result;
    }


    [HttpPut()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Update order status")]
    [EndpointDescription("This function is used by admin to update order status from dashboard")]
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