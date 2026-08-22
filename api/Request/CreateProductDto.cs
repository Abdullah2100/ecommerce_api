using System.ComponentModel.DataAnnotations;
using data.dto.Request;

namespace api.Request;

public class CreateProductApiDto
{
    [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = "name must not be empty")]
    public string Name { get; set; }

    public string Description { get; set; }
    public IFormFile Thumbnail { get; set; }
    public Guid SubcategoryId { get; set; }
    public int Price { get; set; }
    public String Symbol { get; set; }

    public List<CreateProductVariantDto>? ProductVariants { get; set; } = null;
    public List<IFormFile> Images { get; set; }
}