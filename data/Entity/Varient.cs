namespace api.domain.entity;

public class Variant : GeneralSharedInfoWithId
{
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
}