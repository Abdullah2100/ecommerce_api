using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class CreateSubCategoryDto
{
    [Required]
    public string Name { get; set; } = String.Empty;
    [Required]
    public Guid CategoryId { get; set; } 
}