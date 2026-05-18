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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginDto data)
    {
        var result = await deliveryServices.Login(data);

        return result;
    }


    [HttpPost("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDelivery()
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await deliveryServices.GetDelivery(id);

        return result;
    }


    [HttpPut()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateDeliveryInfo([FromForm] UpdateDeliveryDto delivery)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await deliveryServices.UpdateDelivery(delivery, id);

        return result;
    }


    [HttpGet("all")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDelivery(int pageNumber)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await deliveryServices.GetDeliveries(id, pageNumber, 25);

        return result;
    }


    [HttpPut("{status:bool}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateDeliveryStatus(bool status)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await deliveryServices.UpdateDeliveryStatus(id, status);

        return result;
    }


    [HttpGet()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status200OK)]
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


    [HttpGet("me")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status200OK)]
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderDeliveryId(Guid orderId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices.SubmitOrderToDelivery(orderId, id);

        return result;
    }


    [HttpDelete("{orderId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RenameOrderBelongToDelivery(Guid orderId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices.CancelOrderFromDelivery(orderId, id);

        return result;
    }
}