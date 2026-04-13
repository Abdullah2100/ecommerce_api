using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class UpdateVariantDto
{
    [Required]
    public Guid Id { get; set; }
        
    [StringLength(maximumLength: 40 , ErrorMessage = "Enter Valide Name")]
    public string? Name { get; set; } = null;
}