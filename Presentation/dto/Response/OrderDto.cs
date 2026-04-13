namespace api.Presentation.dto.Response
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }

        public long TotalPrice { get; set; }

        //this form api to  let user know is order paid or not 
        public bool IsAlreadyPayed { get; set; }
        public string Symbol { get; set; }
        public int DeliveryFee { get; set; }
        public String Name { get; set; }
        public String UserPhone { get; set; }
        public string Status { get; set; }
        public List<OrderItemDto>? OrderItems { get; set; } = null;
    }
}