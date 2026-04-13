using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class ForgetPasswordDto
{
    [Required]
    public string Email { get; set; }=string.Empty;
}