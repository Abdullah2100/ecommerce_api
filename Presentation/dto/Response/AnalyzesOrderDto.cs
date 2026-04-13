namespace api.Presentation.dto.Response;

public class AnalyzesOrderDto
{
    public decimal? TotalFee { get; set; }
    public long? TotalOrders { get; set; }
    public decimal? TotalDeliveryDistance { get; set; }
    public long? UsersCount { get; set; }
    public long? ProductCount { get; set; }
}