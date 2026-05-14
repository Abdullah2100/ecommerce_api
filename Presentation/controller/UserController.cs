using api.application.Interface;
using api.Filter;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
[Route("api/User")]
public class UserController(IUserServices userServices ) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("auth/signup")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SignUp([FromBody] SignupDto data)
    {
        var result = await userServices.Signup(data);

        return result;
    }


    [AllowAnonymous]
    [HttpPost("auth/login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginDto data)
    {
        var result = await userServices.Login(data);

        return result;
    }


    [HttpGet("me")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUser()
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.GetMe(id);

        return result;
    }


    [HttpGet()]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromQuery()] int page)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.GetUsers(page, id);

        return result;
    }
    //this to get user per pages like we hav 20 pages of user 25 user at one per page 

    [HttpGet("pages")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserPages()
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.GetUsersPages(id);

        return result;
    }


    [HttpPatch("{userId:guid}/status")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> BlockOrUnBlockUser(Guid userId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.BlockOrUnBlockUser(id, userId);

        return result;
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
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.UpdateUser(userData, id);

        return result;
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
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.AddAddressToUser(address, id);

        return result;
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
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.UpdateUserAddress(address, id);


        return result;
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
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.DeleteUserAddress(addressId, id);


        return result;
    }


    [HttpPatch("address/{addressId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateUserCurrentLocation(Guid addressId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.UpdateUserCurrentAddress(addressId, id);


        return result;
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


        return result;
    }


    [AllowAnonymous]
    [HttpPost("auth/otp/verify")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> VerifyingOtp(
        [FromBody] CreateVerificationDto verification
    )
    {
        var result = await userServices.OtpVerification(verification);


        return result;
    }


    [AllowAnonymous]
    [HttpPost("auth/password-reset")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ReseatPassword([FromBody] CreateRecreatePasswordDto data)
    {
        var result = await userServices.RecreatePassword(data);


        return result;
    }
}