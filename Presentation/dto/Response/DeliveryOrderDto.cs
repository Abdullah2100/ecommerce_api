namespace api.Presentation.dto.Response;

public class DeliveryOrderDto
{
    public Guid Id { get; set; }
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
    public long TotalPrice { get; set; }

    public string Symbol { get; set; }

    //this form api to  let  delivery is order paid or not 
    public bool IsAllreadyPayed { get; set; }
    public int DeliveryFee { get; set; }
    public String Name { get; set; }
    public String UserPhone { get; set; }
    public int Status { get; set; }
    public List<DeliveryOrderItemDto> OrderItems { get; set; }
}