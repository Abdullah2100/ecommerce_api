namespace data.dto.Response;

public class AdminOrderDto
{
    public ICollection<OrderDto>? Orders { get; set; }
    public int pageNum { get; set; } = 1;
}