namespace api.Presentation.dto.Response;

public class DeliveryAnalyseDto
{
    public decimal? DayFee { get; set; } = null;
    public decimal? WeekFee { get; set; } = null;
    public decimal? MonthFee { get; set; } = null;
    public int? DayOrder { get; set; } = null;
    public int? WeekOrder { get; set; } = null;
}