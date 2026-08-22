namespace api.domain.entity;

public class Variant : GeneralSharedInfoWithId
{
    public string Name { get; set; } = string.Empty;
    public ICollection<ProductVariant> ProductVariants { get; set; }
}