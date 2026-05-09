using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface IOrderServices
{
    Task<OrderDto?>> CreateOrder(Guid userId,CreateOrderDto orderDto);
    Task<List<OrderDto>>> GetMyOrders(Guid userId,int pageNum,int pageSize);
    
    //order for admin
    Task<AdminOrderDto?>> GetOrders(Guid userId,int pageNum,int pageSize);

    Task<bool>> UpdateOrderStatus(Guid id, int status);
    
   Task<bool>> DeleteOrder(Guid id,Guid userId);
   
   //delivery 
   Task<List<OrderDto>>> GetOrdersByDeliveryId(Guid deliveryId,int pageNum,int pageSize);
   Task<List<OrderDto>>> GetOrdersNotBelongToDeliveries(Guid deliveryId,int pageNum,int pageSize);
   Task<bool>> SubmitOrderToDelivery(Guid id,Guid deliveryId);
   Task<bool>> CancelOrderFromDelivery(Guid id,Guid deliveryId);
   Task<List<string>>> GetOrdersStatus(Guid adminId);
}