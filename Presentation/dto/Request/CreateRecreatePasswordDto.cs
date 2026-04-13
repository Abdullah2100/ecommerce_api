using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class CreateRecreatePasswordDto
{
    [Required] public string Email { get; set; }=string.Empty;
    [Required] public string Otp { get; set; } = string.Empty;
    [Required] public string Password { get; set; }=string.Empty;
}