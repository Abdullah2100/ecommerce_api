using System.Security.Claims;
using api.application.Interface;
using api.Filter;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
[Route("api/Order")]
public class OrderController(
    IOrderServices orderServices,
    IAuthenticationService authenticationService) : ControllerBase
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
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices.CreateOrder(id, orderDto);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }
    
    [HttpGet("status")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetOrderStatus()
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;
        
        var result = await orderServices.GetOrdersStatus(id);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };  

    }
    
    [HttpGet("{pageNumber}")]
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

        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices.GetOrders(id, pageNumber, 25);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [HttpGet("me/{pageNumber}")]
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

        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices
            .GetMyOrders(
                id,
                pageNumber,
                25);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }

    [HttpGet("deliveries/{pageNumber}")]
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

        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices
            .GetOrdersNotBelongToDeliveries(
                id,
                pageNumber,
                25);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    

    [HttpDelete("{orderId}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteOrders
        (Guid orderId)
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await orderServices
            .DeleteOrder(
                orderId, id);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
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
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await orderServices
            .UpdateOrderStatus(
                orderStatus.Id,
               orderStatus.Status
                );

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }
    
    
    
    
}