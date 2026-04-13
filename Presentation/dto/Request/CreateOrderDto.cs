using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class CreateOrderDto
{
    public Guid paymentId { get; set; }
    [Required] public decimal Longitude { get; set; }
    [Required] public decimal Latitude { get; set; }
    [Required] public long TotalPrice { get; set; }
    public required string Symbol { get; set; }
    public Guid PaymentTypeId { get; set; }
    public String? PaymentId { get; set; } = null;
    [Required] public List<CreateOrderItemDto> Items { get; set; }
}