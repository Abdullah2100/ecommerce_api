using api.application.Services.Interface;
using api.Filter;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller.v1;

[Authorize]
[ApiController]
[Route("api/General")]
public class GeneralController(IGeneralSettingServices generalSettingServices) : ControllerBase
{
    [HttpPost("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Create GeneralSetting")]
    [EndpointDescription("This function used by Admin to create new General Setting like distance fee ber kilo ...etc")]

    public async Task<IActionResult> CreateGeneralSetting(
        [FromBody] GeneralSettingDto generalSetting
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await generalSettingServices.CreateGeneralSetting(
            adminId: id,
            generalSetting
        );
        return result;
    }


    [HttpDelete("{generalSettingId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("delete GeneralSetting")]
    [EndpointDescription("This function used by Admin to delete  General Setting")]
    public async Task<IActionResult> DeleteGeneralSetting(
        Guid generalSettingId
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await generalSettingServices.DeleteGeneralSetting(
            adminId: id,
            id: generalSettingId
        );
        return result;
    }

    [HttpPut("{generalSettingId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("update GeneralSetting")]
    [EndpointDescription("This function used by Admin to create update General Setting")]

    public async Task<IActionResult> UpdateGeneralSetting(
        Guid generalSettingId,
        [FromBody] UpdateGeneralSettingDto generalSetting
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;


        var result = await generalSettingServices.UpdateGeneralSetting(
            adminId: id,
            id: generalSettingId,
            settingDto: generalSetting
        );
        return result;
    }

    [AllowAnonymous]
    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [EndpointName("Get GeneralSetting")]
    [EndpointDescription("This function used to  General Settings ")]

    public async Task<IActionResult> GetGeneralSettings(
        int pageNumber
    )
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var result = await generalSettingServices.GetGeneralSettings(
            pageNum: pageNumber,
            pageSize: 25
        );
        return result;
    }
}