using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public enum EnRole
{
    Admin,
    User,
}
public class SignupDto
{
    [StringLength(maximumLength: 50, ErrorMessage = "Name must not be empty")]
    [Required]
    public string Name { get; set; } = string.Empty;

    [StringLength(maximumLength: 13, MinimumLength = 9, ErrorMessage = "phone must between  9 and 13 characters")]
    [Required]
    [MinLength(9, ErrorMessage = "phone must between 9 and 13 characters")]
    [MaxLength(13, ErrorMessage = "phone must between 9 and 13 characters")]
    public string Phone { get; set; } = string.Empty;

    [Required] public string Email { get; set; } = string.Empty;

    [Required] public string Password { get; set; } = string.Empty;

    public string? DeviceToken { get; set; } = null;
    public EnRole? Role { get; set; } = EnRole.User;
}