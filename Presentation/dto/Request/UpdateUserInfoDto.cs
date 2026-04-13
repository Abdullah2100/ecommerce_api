using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class UpdateUserInfoDto
{
    [StringLength(maximumLength: 50 , ErrorMessage = "Enter Valide Name")]
    public string? Name { get; set; } = null;
        
    [StringLength(maximumLength: 13, ErrorMessage = "Enter Valide Name")]
    public string? Phone { get; set; } = null;
    public IFormFile? Thumbnail { get; set; } = null;
    public string? Password { get; set; } = null;
    public string? NewPassword { get; set; } = null;
       
}