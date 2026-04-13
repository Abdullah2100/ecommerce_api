using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class UpdateCategoryDto
{
    [Required] public Guid Id { get; set; }
        
    [StringLength(maximumLength: 50 , ErrorMessage = "Name must not be empty")]
    public string? Name { get; set; } = null;
    public IFormFile? Image { get; set; } = null;

}