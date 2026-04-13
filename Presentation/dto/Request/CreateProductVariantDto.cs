using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class CreateProductVariantDto
{
    [StringLength(maximumLength:50,MinimumLength =1 ,ErrorMessage= "name must not be empty")]
    public string Name { get; set; }
    public int Percentage { get; set; } = 1;
    public Guid VariantId { get; set; }
}