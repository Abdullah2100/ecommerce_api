using api.Filter;
using business.Services.Interface;
using data.Dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.controller;

[Authorize]
[ApiController]
[Route("api/Product")]
public class ProductController(
    IProductServices productServices,
    IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Store")]
    [EndpointName("get products by storeId")]
    [EndpointDescription("This function is used by store to get their own product  page by page")]
    public async Task<IActionResult> GetProducts(Guid storeId, int pageNumber)
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var result = await productServices.GetProductsByStoreId(storeId, pageNumber, 25);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpGet("/")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "User,Store")]
    [EndpointName("get products by categoryId")]
    [EndpointDescription("This function is used by user to get products by categoryId page by page")]
    public async Task<IActionResult> GetProductsByCategory(Guid categoryId, int pageNumber)
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");
        var result = await productServices.GetProductsByCategoryId(categoryId, pageNumber, 25);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpGet("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "User,Store")]
    [EndpointName("get products by storeId and subCategoryId")]
    [EndpointDescription("This function is used by user to get products by storeId and subcategoryId")]
    public async Task<IActionResult> GetProducts
    (
        Guid storeId,
        Guid subcategoryId,
        int pageNumber
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
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin,User")]
    [EndpointName("get products by page")]
    [EndpointDescription("This function is used by user or admin to get products page by page")]
    public async Task<IActionResult> GetProducts(int pageNumber)
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var result = await productServices.GetProducts(pageNumber, 25);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpGet("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    [EndpointName("get products by page for admin")]
    [EndpointDescription("This function is used by admin to get products page by page")]
    public async Task<IActionResult> GetProductsAdmin
        ([FromQuery] int pageNumber, [FromHeader] string header)
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await productServices.GetProductsForAdmin(
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


    [HttpGet("pages")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    [EndpointName("get products pages num ")]
    [EndpointDescription("This function is used by admin to get product pages num for dashboard")]
    public async Task<IActionResult> GetProductsPagesNum()
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await productServices.GetProductsPagesForAdmin(id, 25);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpPost("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin,Store")]
    [EndpointName("create products")]
    [EndpointDescription("This function is used by admin or store to create new product")]
    public async Task<IActionResult> CreateProduct
    (
        [FromForm] CreateProductDto product
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var rootPath = environment.WebRootPath;
        var result = await productServices.CreateProducts(
            id, product, rootPath);


        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpPut("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin,Store")]
    [EndpointName("update products")]
    [EndpointDescription("This function is used by admin or store to update product info")]
    public async Task<IActionResult> UpdateProduct
    (
        [FromForm] UpdateProductDto product
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var rootPath = environment.WebRootPath;
        var result = await productServices.UpdateProducts(
            id, product, rootPath);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpDelete("{productId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin,Store")]
    [EndpointName("delete products")]
    [EndpointDescription("This function is used by admin or store to delete product")]
    public async Task<IActionResult> DeleteProduct
    (
        Guid productId,
        Guid storeId
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var rootPath = environment.WebRootPath;

        var result = await productServices.DeleteProducts(id, storeId, productId, rootPath);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }
}