namespace api.Presentation.dto.Response;

public class OrderItemsStatusEvent
{
    public Guid OrderId { get; set; }
    public Guid OrderItemId { get; set; }
    public string Status { get; set; }
}