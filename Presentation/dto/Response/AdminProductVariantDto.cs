using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Response;

public class AdminProductVariantDto
{
    [StringLength(maximumLength:50 ,ErrorMessage= "name must not  be empty")]
    public string? Name { get; set; }
    public int Percentage { get; set; }
    public string?  VariantName { get; set; } 
}