using api.application.Services.Interface;
using api.Filter;
using api.Presentation.dto.Request;
using api.Request;
using business.Services.Interface;
using data.dto.Request;
using data.dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.controller;

[Authorize]
[ApiController]
[Route("api/User")]
public class UserController(IUserServices userServices,IWebHostEnvironment environment) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("auth/signup")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointName("Signup")]
    [EndpointDescription("This function is used by User to SignUp")]
    public async Task<IActionResult> SignUp([FromBody] SignupDto data)
    {
        var result = await userServices.Signup(data);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [AllowAnonymous]
    [HttpPost("auth/login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointName("Login")]
    [EndpointDescription("This function is used by User to login")]
    public async Task<IActionResult> Login([FromBody] LoginDto data)
    {
        var result = await userServices.Login(data);

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
    [Authorize(Roles = "User")]
    [EndpointName("Get My User")]
    [EndpointDescription("This function is used by user to get their user info")]
    public async Task<IActionResult> GetMyUser()
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.GetMe(id);

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
    [EndpointName("Get Users")]
    [EndpointDescription("This function is used by Admin to get users page by page")]
    public async Task<IActionResult> GetUsers(int page)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.GetUsers(page, id);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    //this to get user per pages like we hav 20 pages of user 25 user at one per page 
    [HttpGet("pages")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Get Users pages num")]
    [EndpointDescription("This function is used by Admin to get users pages num")]
    public async Task<IActionResult> GetUserPages()
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.GetUsersPages(id);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpPatch("{userId:guid}/status")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin")]
    [EndpointName("Change Users Status")]
    [EndpointDescription("This function is used by Admin to block or unblock userUser")]
    public async Task<IActionResult> BlockOrUnBlockUser(Guid userId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.BlockOrUnBlockUser(id, userId);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpPut("")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "User,Store")]
    [EndpointName("Update Users")]
    [EndpointDescription("This function is used by User to update it own user info")]
    public async Task<IActionResult> UpdateUser(
        [FromForm] UpdateUserInfoDto userData
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.UpdateUser(userData, id,environment.ContentRootPath);

        return result.IsSuccessful switch
        {
            false => new ObjectResult(result.Message) { StatusCode = result.StatusCode },
            _ => new ObjectResult(result.Data) { StatusCode = result.StatusCode }
        };
    }


    [HttpPost("address")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "User")]
    [EndpointName("Add Address")]
    [EndpointDescription("This function is used by User to to Add new Address")]
    public async Task<IActionResult> AddNewUserAddress(
        [FromBody] CreateAddressDto address
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.AddAddressToUser(address, id);

        return result.IsSuccessful switch
        {
            false=> new ObjectResult(result.Message){StatusCode = result.StatusCode},
            _=> new ObjectResult(result.Data){StatusCode = result.StatusCode}
        };    }

    [HttpPut("address")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "User")]
    [EndpointName("Update Users Current Address")]
    [EndpointDescription("This function is used by User to update curren address for them")]
    public async Task<IActionResult> UpdateUserLocation(
        [FromBody] UpdateAddressDto address
    )
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.UpdateUserAddress(address, id);


        return result.IsSuccessful switch
        {
            false=> new ObjectResult(result.Message){StatusCode = result.StatusCode},
            _=> new ObjectResult(result.Data){StatusCode = result.StatusCode}
        };    }

    [HttpDelete("address/{addressId}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "User")]
    [EndpointName("delete Users")]
    [EndpointDescription("This function is used by User to delete address")]
    public async Task<IActionResult> DeleteUserLocation(Guid addressId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.DeleteUserAddress(addressId, id);


        return result.IsSuccessful switch
        {
            false=> new ObjectResult(result.Message){StatusCode = result.StatusCode},
            _=> new ObjectResult(result.Data){StatusCode = result.StatusCode}
        };    }


    [HttpPatch("address/{addressId:guid}")]
    [GetUserIdFromUserClaims]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "User")]
    [EndpointName("Update Users address Info")]
    [EndpointDescription("This function is used by User to update then address info")]
    public async Task<IActionResult> UpdateUserCurrentLocation(Guid addressId)
    {
        var id = HttpContext.Items["id"] as Guid? ?? Guid.Empty;

        var result = await userServices.UpdateUserCurrentAddress(addressId, id);


        return result.IsSuccessful switch
        {
            false=> new ObjectResult(result.Message){StatusCode = result.StatusCode},
            _=> new ObjectResult(result.Data){StatusCode = result.StatusCode}
        };    }


    [AllowAnonymous]
    [HttpPost("auth/otp/generate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointName("Generate Otp")]
    [EndpointDescription("This function is used to generate otp")]
    public async Task<IActionResult> GenerateOtp(
        [FromBody] ForgetPasswordDto otp
    )
    {
        var result = await userServices.GenerateOtp(otp);


        return result.IsSuccessful switch
        {
            false=> new ObjectResult(result.Message){StatusCode = result.StatusCode},
            _=> new ObjectResult(result.Data){StatusCode = result.StatusCode}
        };    }


    [AllowAnonymous]
    [HttpPost("auth/otp/verify")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointName("Verifying Otp")]
    [EndpointDescription("This function is used to verifying otp")]
    public async Task<IActionResult> VerifyingOtp(
        [FromBody] CreateVerificationDto verification
    )
    {
        var result = await userServices.OtpVerification(verification);


        return result.IsSuccessful switch
        {
            false=> new ObjectResult(result.Message){StatusCode = result.StatusCode},
            _=> new ObjectResult(result.Data){StatusCode = result.StatusCode}
        };    }


    [AllowAnonymous]
    [HttpPost("auth/password-reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointName("Reseate password")]
    [EndpointDescription("This function is used to reseat password")]
    public async Task<IActionResult> ReseatPassword([FromBody] CreateRecreatePasswordDto data)
    {
        var result = await userServices.RecreatePassword(data);


        return result.IsSuccessful switch
        {
            false=> new ObjectResult(result.Message){StatusCode = result.StatusCode},
            _=> new ObjectResult(result.Data){StatusCode = result.StatusCode}
        };    }
}