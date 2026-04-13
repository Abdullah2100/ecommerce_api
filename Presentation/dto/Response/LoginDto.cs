using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Response;

public class LoginDto
{
    [StringLength(maximumLength: 50,  ErrorMessage = "username must not be empty")]
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required] public string Password { get; set; } = string.Empty;

    public string? DeviceToken { get; set; } = null;
}