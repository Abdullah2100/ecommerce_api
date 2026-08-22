using System.ComponentModel.DataAnnotations;

namespace data.dto.Request;

public class ForgetPasswordDto
{
    [Required] public string Email { get; set; } = string.Empty;
}