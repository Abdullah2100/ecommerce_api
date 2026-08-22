using api.Filter;
using api.shared.signalr;
using business.Services.Interface;
using data.dto.Request;
using data.dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace api.controller;

[Authorize]
[ApiController]
[Route("api/Delivery")]
public class DeliveryController(
    IDeliveryServices deliveryServices,
    IOrderServices orderServices,
    IWebHostEnvironment environment,
    IHubContext<OrderHub> hubContext
    )
    : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginDto data)
    {
        var result = await deliveryServices.Login(data);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpPost("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin,Store")]
    [EndpointName("Create new Delivery")]
    [EndpointDescription("This function used by admin or store owner to create delivery")]
    public async Task<IActionResult> CreateDelivery
        ([FromForm] CreateDeliveryDto delivery)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await deliveryServices.CreateDelivery(
            id,
            delivery, environment.ContentRootPath);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpGet("me")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Store")]
    [EndpointName("Get Deliveries for store")]
    [EndpointDescription("This function used by  store owner to get deliveries belong to them")]
    public async Task<IActionResult> GetDelivery()
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await deliveryServices.GetDelivery(id);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpPut()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Delivery")]
    [EndpointName("Update Deliveries info")]
    [EndpointDescription("This function used by delivery to update there info")]
    public async Task<IActionResult> UpdateDeliveryInfo([FromForm] UpdateDeliveryDto delivery)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await deliveryServices.UpdateDelivery(delivery, id, environment.ContentRootPath);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpGet("all")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Store")]
    [EndpointName("Get Deliveries belong to store")]
    [EndpointDescription("This function used by store owner to get deliveries belong to them by pagination")]
    public async Task<IActionResult> GetDelivery(int pageNumber)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await deliveryServices.GetDeliveries(id, pageNumber, 25);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpPut("{status:bool}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin,Delivery")]
    [EndpointName("block or unblock delivery")]
    [EndpointDescription("This function used by store owner or admin to block or unblock delivery")]
    public async Task<IActionResult> UpdateDeliveryStatus(bool status)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await deliveryServices.UpdateDeliveryStatus(id, status);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpGet()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Delivery")]
    [EndpointName("Get order not own by any delivery")]
    [EndpointDescription("This function used by delivery to get order not own by any another deliveries")]
    public async Task<IActionResult> GetOrderNotTookByDelivery
    (
        int pageNumber = 1
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await orderServices
            .GetOrdersNotBelongToDeliveries(id, pageNumber, 25);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpGet("my/orders")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Delivery")]
    [EndpointName("Get order belong to me")]
    [EndpointDescription("This function used by delivery to get order own by me delivery by page")]
    public async Task<IActionResult> GetOrderBelongToMe(int pageNumber = 1)
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await orderServices.GetOrdersByDeliveryId(
            id, pageNumber, 25);
        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpPatch("{orderId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Delivery")]
    [EndpointName("update order delivery id ")]
    [EndpointDescription("This function used by delivery submit order to specific delivery")]
    public async Task<IActionResult> UpdateOrderDeliveryId(Guid orderId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices.SubmitOrderToDelivery(
            orderId,
            id,
            async (value) =>
            {
                await hubContext.Clients.All.SendAsync("orderGettingByDelivery", value);

            },
             async (value) =>
            {
         await hubContext.Clients.All.SendAsync("orderStatus", value);

            });

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpDelete("{orderId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Delivery")]
    [EndpointName("cancel order delivery")]
    [EndpointDescription("This function used by delivery to remove delivery Id from order")]
    public async Task<IActionResult> RenameOrderBelongToDelivery(Guid orderId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices.CancelOrderFromDelivery(
            orderId, 
            id ,
            async (value) =>
            {
                        await hubContext.Clients.All.SendAsync("createdOrder", value);

            });

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }
}