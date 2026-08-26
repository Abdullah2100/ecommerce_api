namespace api.domain.entity;

public class ProductVariant : GeneralSharedInfoWithId
{
    public int Percentage { get; set; }
    public Guid VariantId { get; set; }
    public string Name { get; set; }
    public Guid ProductId { get; set; }
    public virtual Variant? Variant { get; set; } = null;
    public virtual Product? Product { get; set; } = null;
    public virtual ICollection<OrderProductsVariant> OrderProductsVariants { get; set; } = new List<OrderProductsVariant>();
}