namespace api.Presentation.dto.Response;

public class DeliveryOrderItemDto
    
{

    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DeliveryAddressDto? Address { get; set; } 
    public OrderProductDto? Product { get; set; }
    public List<OrderVariantDto>? ProductVariant { get; set; } = null;
    public String OrderItemStatus { get; set; } = "";
}