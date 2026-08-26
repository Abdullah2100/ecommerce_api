using System.ComponentModel.DataAnnotations;

namespace api.domain.entity;

public enum EnOrderItemStatus
{
    Cancelled,
    InProgress,
    Excepted,
    ReceivedByDelivery
}

public class OrderItem : GeneralShredInfo
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public Guid StoreId { get; set; }
    public virtual Order Order { get; set; } = null!;
    public virtual Store Store { get; set; }= null!;
    public virtual Product Product { get; set; }= null!;
    public virtual ICollection<OrderProductsVariant> OrderProductsVariants { get; set; } = new List<OrderProductsVariant>();
    public  virtual EnOrderItemStatus Status { get; set; } = EnOrderItemStatus.InProgress;
}