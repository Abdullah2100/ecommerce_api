namespace api.Presentation.dto.Response;

public class AdminOrderDto
{
    public List<OrderDto>? Orders { get; set; }
    public int pageNum { get; set; } = 1;
}