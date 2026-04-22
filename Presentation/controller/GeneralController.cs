using System.Security.Claims;
using api.application.Interface;
using api.Filter;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
[Route("api/General")]
public class GeneralController(
    IGeneralSettingServices generalSettingServices,
    IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateGeneralSetting(
        [FromBody] GeneralSettingDto generalSetting
    )
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await generalSettingServices.CreateGeneralSetting(
            adminId:id,
            generalSetting
            );
        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };   
    }


    [HttpDelete("{generalSettingId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteGeneralSetting(
        Guid genralSettingId 
    )
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

       
        var result = await generalSettingServices.DeleteGeneralSetting(
            adminId:id,
            id:genralSettingId
        );
        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };   
    }

    [HttpPut("{generalSettingId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGeneralSetting(
        Guid genralSettingId ,
        [FromBody] UpdateGeneralSettingDto generalSetting
    )
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        
        var result = await generalSettingServices.UpdateGeneralSetting(
            adminId:id,
            id:genralSettingId,
            settingDto:generalSetting
        );
        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };   
        
     
    }

    [AllowAnonymous]
    [HttpGet("{pageNumber:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetGeneralSettings(
         int pageNumber
    )
    {
        if (pageNumber < 1)
            return BadRequest("رقم الصفحة لا بد ان تكون اكبر من الصفر");

        var result = await generalSettingServices.GetGeneralSettings(
            pageNum:pageNumber,
            pageSize:25
        );
        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };    
    }


  
    
    
}