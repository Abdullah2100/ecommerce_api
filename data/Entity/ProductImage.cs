using System.ComponentModel.DataAnnotations;

namespace api.domain.entity;

public class ProductImage : GeneralSharedInfoWithId
{
    public string Path { get; set; }
    public Guid ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
}