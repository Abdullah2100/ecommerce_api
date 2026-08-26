using System.ComponentModel.DataAnnotations;

namespace data.dto.Request;

public class CreateVerificationDto
{
    [Required] public string Email { get; set; } = String.Empty;
    [Required] public string Otp { get; set; } = String.Empty;
}