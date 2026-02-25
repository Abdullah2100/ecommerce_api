using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto
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

    public class AdminOrderDto
    {
        public List<OrderDto>? Orders { get; set; }
        public int pageNum { get; set; } = 1;
    }

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

    public class CreateOrderDto
    {
        public Guid paymentId { get; set; }
        [Required] public decimal Longitude { get; set; }
        [Required] public decimal Latitude { get; set; }
        [Required] public long TotalPrice { get; set; }
        public required string Symbol { get; set; }
        public Guid PaymentTypeId { get; set; }
        public String? PaymentId { get; set; } = null;
        [Required] public List<CreateOrderItemDto> Items { get; set; }
    }

    public class OrderTookByEvent
    {
        public Guid Id { get; set; }
        public Guid DeliveryId { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        public Guid Id { get; set; }
        public int Status { get; set; }
    }

    public class UpdateOrderStatusEventDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
    }


    public class AnalyzesOrderDto
    {
        public decimal? TotalFee { get; set; }
        public long? TotalOrders { get; set; }
        public decimal? TotalDeliveryDistance { get; set; }
        public long? UsersCount { get; set; }
        public long? ProductCount { get; set; }
    }
}