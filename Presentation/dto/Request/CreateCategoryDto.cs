using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class CreateCategoryDto
{
    [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "Name must not be empty")]
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required] public IFormFile Image { get; set; }
}