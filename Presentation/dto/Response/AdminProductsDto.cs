using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Response;

public class AdminProductsDto
{
    public Guid Id { get; set; }
        
    [StringLength(maximumLength: 100 , ErrorMessage = "name must not be empty")]
    public string Name { get; set; }
        
    public string Description { get; set; }
    public string  Thumbnail { get; set; }
    public string Subcategory { get; set; }
    public string StoreName { get; set; }
    public int Price { get; set; }
    public String Symbol { get; set; } 

    public List<List<AdminProductVariantDto>>? ProductVariants { get; set; }
    public List<string> ProductImages { get; set; } 
}