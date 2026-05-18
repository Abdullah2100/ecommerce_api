using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Interface;

public interface IOrderServices
{
    Task<IActionResult> CreateOrder(Guid userId, CreateOrderDto orderDto);
    Task<IActionResult> GetMyOrders(Guid userId, int pageNum, int pageSize);

    //order for admin
    Task<IActionResult> GetOrders(Guid userId, int pageNum, int pageSize);

    Task<IActionResult> UpdateOrderStatus(Guid id, int status);

    Task<IActionResult> DeleteOrder(Guid id, Guid userId);

    //delivery 
    Task<IActionResult> GetOrdersByDeliveryId(Guid deliveryId, int pageNum, int pageSize);
    Task<IActionResult> GetOrdersNotBelongToDeliveries(Guid deliveryId, int pageNum, int pageSize);
    Task<IActionResult> SubmitOrderToDelivery(Guid id, Guid deliveryId);
    Task<IActionResult> CancelOrderFromDelivery(Guid id, Guid deliveryId);
    Task<IActionResult> GetOrdersStatus(Guid adminId);
}