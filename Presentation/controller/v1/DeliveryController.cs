using System.Security.Claims;
using api.application.Interface;
using api.application.Services.Interface;
using api.Filter;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
[Route("api/Delivery")]
public class DeliveryController(
    IDeliveryServices deliveryServices,
    IOrderServices orderServices)
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

        return result;
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
            delivery);

        return result;
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

        return result;
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


        var result = await deliveryServices.UpdateDelivery(delivery, id);

        return result;
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

        return result;
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

        return result;
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

        return result;
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

        return result;
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

        var result = await orderServices.SubmitOrderToDelivery(orderId, id);

        return result;
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

        var result = await orderServices.CancelOrderFromDelivery(orderId, id);

        return result;
    }
}