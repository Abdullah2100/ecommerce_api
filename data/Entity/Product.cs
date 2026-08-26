using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

public class Product : GeneralShredInfo
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Thumbnail { get; set; }
    public Guid SubcategoryId { get; set; }
    public Guid StoreId { get; set; }
    public int Price { get; set; }
    public int? Quantity { get; set; } = null;
    public String Symbol { get; set; }
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>;
    public virtual SubCategory SubCategory { get; set; } = null!;
    public virtual Store Store { get; set; } = null!;
}