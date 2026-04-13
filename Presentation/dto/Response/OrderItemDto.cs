namespace api.Presentation.dto.Response
{


    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string OrderStatusName { get; set; }
        public List<AddressWithTitleDto>? Address { get; set; } = null;
        public OrderProductDto? Product { get; set; }
        public List<OrderVariantDto>? ProductVariant { get; set; } = null;
        public String OrderItemStatus { get; set; } = ""; 
    }
}