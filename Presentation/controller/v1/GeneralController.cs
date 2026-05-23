using api.application.Interface;
using api.application.Services.Interface;
using api.Filter;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller;

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