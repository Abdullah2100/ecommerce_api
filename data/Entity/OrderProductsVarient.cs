using System.ComponentModel.DataAnnotations;

namespace api.domain.entity;

public class OrderProductsVariant : GeneralShredInfo
{
    public Guid ProductVariantId { get; set; }
    public Guid OrderItemId { get; set; }
    public Guid Name { get; set; }
    public virtual ProductVariant? ProductVariant { get; set; } = null;
    public virtual OrderItem? OrderItem { get; set; } = null;
}