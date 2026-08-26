namespace data.dto.Response
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string OrderStatusName { get; set; } = "";
        public ICollection<AddressWithTitleDto>? Address { get; set; } = null;
        public OrderProductDto? Product { get; set; }
        public ICollection<OrderVariantDto>? ProductVariant { get; set; } = null;
        public String OrderItemStatus { get; set; } = "";
    }
}