using System.Security.Claims;
using api.application.Interface;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
[Route("api/Product")]
public class ProductController(
    IProductServices productServices,
    IAuthenticationService authenticationService
) : ControllerBase
{
    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts
        ([FromQuery] Guid storeId, [FromQuery] int pageNumber)
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var result = await productServices.GetProductsByStoreId(storeId, pageNumber, 25);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductsByCategory
    (
        [FromQuery] Guid categoryId, [FromQuery] int pageNumber
    )
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");
        var result = await productServices.GetProductsByCategoryId(categoryId, pageNumber, 25);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts
    (
        [FromQuery] Guid storeId,
        [FromQuery] Guid subcategoryId,
        [FromQuery] int pageNumber
    )
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var result = await productServices.GetProducts(
            storeId,
            subcategoryId,
            pageNumber,
            25);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts
        ([FromQuery] int pageNumber)
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var result = await productServices.GetProducts(pageNumber, 25);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductsAdmin
        ([FromQuery] int pageNumber, [FromHeader] string header)
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        StringValues authorizationHeader = HttpContext.Request.Headers["Authorization"];
        Claim? id = authenticationService.GetPayloadFromToken("id",
            authorizationHeader.ToString().Replace("Bearer ", ""));

        Guid adminId = Guid.Empty;
        if (Guid.TryParse(id?.Value.ToString(), out Guid outId))
        {
            adminId = outId;
        }

        if (adminId == Guid.Empty)
        {
            return Unauthorized("هناك مشكلة في التحقق");
        }

        var result = await productServices.GetProductsForAdmin(
            adminId,
            pageNumber,
            25
        );

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [HttpGet("pages")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductsPagesNum()
    {
        StringValues authorizationHeader = HttpContext.Request.Headers["Authorization"];
        Claim? id = authenticationService.GetPayloadFromToken("id",
            authorizationHeader.ToString().Replace("Bearer ", ""));

        Guid adminId = Guid.Empty;
        if (Guid.TryParse(id?.Value.ToString(), out Guid outId))
        {
            adminId = outId;
        }

        if (adminId == Guid.Empty)
        {
            return Unauthorized("هناك مشكلة في التحقق");
        }

        var result = await productServices.GetProductsPagesForAdmin(adminId, 25);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateProduct
    (
        [FromForm] CreateProductDto product
    )
    {
        StringValues authorizationHeader = HttpContext.Request.Headers["Authorization"];
        Claim? id = authenticationService.GetPayloadFromToken("id",
            authorizationHeader.ToString().Replace("Bearer ", ""));

        Guid userId = Guid.Empty;
        if (Guid.TryParse(id?.Value.ToString(), out Guid outId))
        {
            userId = outId;
        }

        if (userId == Guid.Empty)
        {
            return Unauthorized("هناك مشكلة في التحقق");
        }

        var result = await productServices.CreateProducts(
            userId, product);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [HttpPut("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProduct
    (
        [FromForm] UpdateProductDto product
    )
    {
        StringValues authorizationHeader = HttpContext.Request.Headers["Authorization"];
        Claim? id = authenticationService.GetPayloadFromToken("id",
            authorizationHeader.ToString().Replace("Bearer ", ""));

        Guid userId = Guid.Empty;
        if (Guid.TryParse(id?.Value.ToString(), out Guid outId))
        {
            userId = outId;
        }

        if (userId == Guid.Empty)
        {
            return Unauthorized("هناك مشكلة في التحقق");
        }

        var result = await productServices.UpdateProducts(
            userId, product);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [HttpDelete("{storeId:guid}/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct
    (
        Guid storeId,
        Guid productId
    )
    {
        StringValues authorizationHeader = HttpContext.Request.Headers["Authorization"];
        Claim? id = authenticationService.GetPayloadFromToken("id",
            authorizationHeader.ToString().Replace("Bearer ", ""));

        Guid userId = Guid.Empty;
        if (Guid.TryParse(id?.Value.ToString(), out Guid outId))
        {
            userId = outId;
        }

        if (userId == Guid.Empty)
        {
            return Unauthorized("هناك مشكلة في التحقق");
        }

        var result = await productServices.DeleteProducts(userId, storeId, productId);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }
}