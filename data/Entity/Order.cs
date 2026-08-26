using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

public class Order : GeneralShredInfo
{
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
    public Guid UserId { get; set; }
    public long TotalPrice { get; set; }
    public string Symbol { get; set; }
    public int Status { get; set; }
    public int DistanceToUser { get; set; } = 0;
    public int DistanceFee { get; set; } = 0;
    public bool IsFail { get; set; } = false;
    public Guid PaymentTypeId { get; set; }
    public Guid? DeliveryId { get; set; } = null;
    public virtual PaymentType PaymentType { get; set; } = null!;
    public virtual User User { get; set; } = null!;
    public virtual Delivery? DeliveredBy { get; set; } = null;
    public virtual ICollection<OrderItem> Items { get; set; } = new ICollection<OrderItem>();
}