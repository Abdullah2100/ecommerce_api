using api.application.Services.Interface;
using api.Filter;
using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller.v1;

[Authorize]
[GetUserIdFromUserClaims]
[ApiController]
[Route("api/OrderItems")]
public class OrderItemController(IOrderItemServices orderItemServices) : ControllerBase
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

        return result;
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
                orderItemStatusDto);

        return result;
    }
}