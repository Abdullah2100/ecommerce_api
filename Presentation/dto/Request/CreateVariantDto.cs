using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class CreateVariantDto
{
    [StringLength(maximumLength: 40, MinimumLength = 3, ErrorMessage = "Enter Valide Name")]
    [Required]
    public string Name { get; set; } = string.Empty;
}