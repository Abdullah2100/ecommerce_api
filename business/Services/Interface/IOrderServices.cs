using api.application;
using data.dto.Request;
using data.dto.Response;

namespace business.Services.Interface;

public interface IOrderServices
{
    Task<Result> CreateOrder(Guid userId, CreateOrderDto orderDto,Action<OrderDto> sendMessage);
    Task<Result> GetMyOrders(Guid userId, int pageNum, int pageSize);

    //order for admin
    Task<Result> GetOrders(Guid userId, int pageNum, int pageSize);

    Task<Result> UpdateOrderStatus(Guid id, int status,Action<UpdateOrderStatusEventDto>sendMessage);

    Task<Result> DeleteOrder(Guid id, Guid userId);

    //delivery 
    Task<Result> GetOrdersByDeliveryId(Guid deliveryId, int pageNum, int pageSize);
    Task<Result> GetOrdersNotBelongToDeliveries(Guid deliveryId, int pageNum, int pageSize);
    Task<Result> SubmitOrderToDelivery(
        Guid id, 
        Guid deliveryId,
        Action<OrderTookByEvent>sendMessage1,
        Action<UpdateOrderStatusEventDto>sendMessage2);
    Task<Result> CancelOrderFromDelivery(
        Guid id, 
        Guid deliveryId,
        Action<OrderDto>sendMessage);
    Task<Result> GetOrdersStatus(Guid adminId);
}