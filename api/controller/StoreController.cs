using api.application.Services.Interface;
using api.Filter;
using api.shared.signalr;
using business.Services.Interface;
using data.dto.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace api.controller;

[Authorize]
[ApiController]
[Route("api/store")]
public class StoreController(
    IStoreServices storeServices,
    IBannerServices bannerServices,
    ISubCategoryServices subCategoryServices,
    IHubContext<BannerHub> hubContext,
    IHubContext<StoreHub> hubStoreContext,
    IWebHostEnvironment environment
)
    : ControllerBase
{
    [HttpPost("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "User,Admin")]
    [EndpointName("Create Store")]
    [EndpointDescription("This function is used by admin or user to create store")]
    public async Task<IActionResult> CreateNewStore(
        [FromForm] CreateStoreDto store)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;
        var rootPath = environment.WebRootPath;

        var result = await storeServices.CreateStore(store, id,rootPath);

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
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Store,Admin")]
    [EndpointName("Update Store")]
    [EndpointDescription("This function is used by admin or user to update store info")]
    public async Task<IActionResult> UpdateStore(
        [FromForm] UpdateStoreDto store)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var rootPath = environment.WebRootPath;
        var result = await storeServices.UpdateStore(store, id,rootPath);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpPut("{storeId:guid}/status")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Update Store status")]
    [EndpointDescription("This function is used by admin update store status")]
    public async Task<IActionResult> UpdateStoreStatus(
        Guid storeId
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await storeServices.UpdateStoreStatus(id, storeId, async (value) =>
        {
            await hubStoreContext.Clients.All.SendAsync("storeStatus", value);

        });


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
    [Authorize(Roles = "Store,User")]
    [EndpointName("Get  Store info")]
    [EndpointDescription("This function is used by User or Store to  get Store Info")]
    public async Task<IActionResult> GetMyStore()
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await storeServices.GetStoreByUserId(id);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }

    [HttpGet("{storeId:guid}/pages")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Get Store Pages num")]
    [EndpointDescription("This function is used by admin To Get Store Pages num")]
    public async Task<IActionResult> GetStoresPages(Guid storeId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await storeServices.GetStorePage(id, 20);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpGet("{storeId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "User,Admin")]
    [EndpointName("Get Store")]
    [EndpointDescription("This function is used by admin or user to get Store Info by StoreId")]
    public async Task<IActionResult> GetStoreById(Guid storeId)
    {
        var result = await storeServices.GetStoreByStoreId(storeId);

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
    [Authorize(Roles = "Admin")]
    [EndpointName("Get Stores by Pages")]
    [EndpointDescription("This function is used by admin to get stores page by page")]
    public async Task<IActionResult> GetStores(int page = 1)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await storeServices.GetStores(id, page, 25);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }

    //this or admin page to get name of store while typing 
    [HttpGet("search/{prefix:regex(^[[\\p{{L}}]]+$)}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "User,Admin")]
    [EndpointName("Get Store")]
    [EndpointDescription("This function is used by admin or user to get Store by name")]
    public async Task<IActionResult> GetStores(string prefix)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await storeServices.GetStores(id, prefix, 25);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }

    [HttpPost("{storeId:guid}/banners")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Store")]
    [EndpointName("Create Banner For Store")]
    [EndpointDescription("This function is used by Store to Create Banner")]
    public async Task<IActionResult> CreateBanner(
        Guid storeId, [FromForm] CreateBannerDto banner
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await bannerServices.CreateBanner(id, banner, environment.ContentRootPath,
            async (value) => { await hubContext.Clients.All.SendAsync("createdBanner", value); });

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }

    [HttpDelete("{storeId:guid}/banners/{bannerId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Store")]
    [EndpointName("Delete Banner")]
    [EndpointDescription("This function is used by Store to delete Banner")]
    public async Task<IActionResult> DeleteBanner(
        Guid storeId, Guid bannerId
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await bannerServices
            .DeleteBanner(bannerId, id, environment.ContentRootPath,
                async void (value) => { await hubContext.Clients.All.SendAsync("deletedBanner", id); });

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }

    [HttpGet("{storeId:guid}/banners")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Store,User")]
    [EndpointName("Get banners by storeId")]
    [EndpointDescription("This function is used by Store to get banners by storeId")]
    public async Task<IActionResult> GetBanner(
        Guid storeId, int pageNumber
    )
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var result = await bannerServices
            .GetBanners(storeId, pageNumber, 25);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }

    [HttpPost("{storeId:guid}/subCategories")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Store")]
    [EndpointName("Create  Sub Category")]
    [EndpointDescription("This function is used by Store to Create  Sub Category")]
    public async Task<IActionResult> CreatSubCategory(
        Guid storeId,
        [FromBody] CreateSubCategoryDto subCategory)
    {
        var result = await subCategoryServices.CreateSubCategory(storeId, subCategory);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }

    [HttpPut("{storeId:guid}/subCategories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Store")]
    [EndpointName("Update  Sub Category")]
    [EndpointDescription("This function is used by Store to update  Sub Category")]
    public async Task<IActionResult> UpdateSubCategory(
        Guid storeId,
        [FromBody] UpdateSubCategoryDto subCategory)
    {
        var result = await subCategoryServices.UpdateSubCategory(storeId, subCategory);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }

    [HttpDelete("{storeId:guid}/subCategories/{subCategoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Store")]
    [EndpointName("Delete  Sub Category")]
    [EndpointDescription("This function is used by Store to delete Sub Category")]
    public async Task<IActionResult> DeleteSubCategory
        (Guid storeId, Guid subCategoryId)

    {
        var result = await subCategoryServices.DeleteSubCategory(
            storeId: storeId,
            id: subCategoryId);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }

    [HttpGet("{storeId:guid}/subCategories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Store,User")]
    [EndpointName("get Sub Categories")]
    [EndpointDescription("This function is used by Store or User to get Sub Categories page by page")]
    public async Task<IActionResult> GetSubCategory(Guid storeId, int page)
    {
        var result = await subCategoryServices.GetSubCategories(
            storeId, page, 25);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }
}