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
[Route("api/User")]
public class UserController(
    IUserServices userServices,
    IAuthenticationService authenticationService
) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("auth/signup")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignUp([FromBody] SignupDto data)
    {
        var result = await userServices.Signup(data);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [AllowAnonymous]
    [HttpPost("auth/login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginDto data)
    {
        var result = await userServices.Login(data);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [HttpGet("me")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUser()
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.GetMe(id);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [HttpGet()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromQuery()] int page)
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.GetUsers(page, id);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }
    //this to get user per pages like we hav 20 pages of user 25 user at one per page 

    [HttpGet("pages")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserPages()
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.GetUsersPages(id);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [HttpPatch("{userId:guid}/status")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> BlockOrUnBlockUser(Guid userId)
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.BlockOrUnBlockUser(id, userId);

        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [HttpPut("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUser(
        [FromForm] UpdateUserInfoDto userData
    )
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.UpdateUser(userData, id);

        return result.IsSuccessful switch
        {
            true => StatusCode(result
                .StatusCode, result.Data),
            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [HttpPost("address")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> AddNewUserAddress(
        [FromBody] CreateAddressDto address
    )
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.AddAddressToUser(address, id);

        return result.IsSuccessful switch
        {
            true => StatusCode(result
                .StatusCode, result.Data),

            _ => StatusCode(result.StatusCode, result.Message)
        };
    }

    [HttpPut("address")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserLocation(
        [FromBody] UpdateAddressDto address
    )
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.UpdateUserAddress(address, id);


        return result.IsSuccessful switch
        {
            true => StatusCode(result
                .StatusCode, result.Data),

            _ => StatusCode(result.StatusCode, result.Message)
        };
    }

    [HttpDelete("address/{addressId}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteUserLocation(
        Guid addressId
    )
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.DeleteUserAddress(addressId, id);


        return result.IsSuccessful switch
        {
            true => StatusCode(result
                .StatusCode, result.Data),

            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [HttpPatch("address/{addressId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateUserCurrentLocation(Guid addressId)
    {
        Guid id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.UpdateUserCurrentAddress(addressId, id);


        return result.IsSuccessful switch
        {
            true => StatusCode(result
                .StatusCode, result.Data),

            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [AllowAnonymous]
    [HttpPost("auth/otp/generate")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GenerateOtp(
        [FromBody] ForgetPasswordDto otp
    )
    {
        var result = await userServices.GenerateOtp(otp);


        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),

            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [AllowAnonymous]
    [HttpPost("auth/otp/verify")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> VerifingOtp(
        [FromBody] CreateVerificationDto verification
    )
    {
        var result = await userServices.OtpVerification(verification);


        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),

            _ => StatusCode(result.StatusCode, result.Message)
        };
    }


    [AllowAnonymous]
    [HttpPost("auth/password-reset")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReseatPassword([FromBody] CreateRecreatePasswordDto data)
    {
        var result = await userServices.ReseatePassword(data);


        return result.IsSuccessful switch
        {
            true => StatusCode(result.StatusCode, result.Data),

            _ => StatusCode(result.StatusCode, result.Message)
        };
    }
}