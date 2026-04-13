using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class UpdateProductDto 
{
    [Required]public Guid Id { get; set; }
        
    [StringLength(maximumLength: 100 , ErrorMessage = "name must not be empty")]
    public string? Name { get; set; }= null;
        
    public string? Description { get; set; }= null;
    public IFormFile?  Thumbnail { get; set; }= null;
    public Guid? SubcategoryId { get; set; }= null;
    [Required] public Guid StoreId { get; set; }
    public int? Price { get; set; }= null;
    public String? Symbol { get; set; } = null; 

    public List<CreateProductVariantDto>? ProductVariants { get; set; } = null;
    public List<CreateProductVariantDto>? DeletedProductVariants { get; set; } = null;
    public List<IFormFile>? Images { get; set; }= null;
    public List<string>? Deletedimages { get; set; }= null;

     
}