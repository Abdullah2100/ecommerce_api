using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class CreateOrderItemDto
{
    [Required] public Guid StoreId { get; set; }
    [Required] public decimal Price { get; set; }
    [Required] public int Quantity { get; set; } = 1;
    [Required] public Guid ProductId { get; set; }

    public List<Guid>? ProductVariant { get; set; } = null;
}