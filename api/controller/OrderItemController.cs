using api.application.Services.Interface;
using api.Filter;
using api.shared.signalr;
using business.Services.Interface;
using data.dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace api.Presentation.controller;

[Authorize]
[GetUserIdFromUserClaims]
[ApiController]
[Route("api/OrderItems")]
public class OrderItemController(
    IOrderItemServices orderItemServices,
    IHubContext<OrderItemHub> hubContext) : ControllerBase
{
    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Store")]
    [EndpointName("Get OrderItems for Store")]
    [EndpointDescription("This function is used by store to get orderItem that order it by users")]
    public async Task<IActionResult> GetOrdersItemForStore(int pageNumber = 1)
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

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }

    [HttpPut("status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Store,Delivery")]
    [EndpointName("Update orderItems status")]
    [EndpointDescription("This function is used by store owner or delivery to change the orderItems status")]
    public async Task<IActionResult> UpdateOrderItemStatus
        ([FromBody] UpdateOrderItemStatusDto orderItemStatusDto)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderItemServices
            .UpdateOrderItemsStatus(
                id,
                orderItemStatusDto,
                async (value) => { await hubContext.Clients.All.SendAsync("orderItemsStatusChange", value); });

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }
}