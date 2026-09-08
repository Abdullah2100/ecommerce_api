using api.domain.entity;
using data.dto.Request;
using data.dto.Response;

namespace data.Interface;

public interface IOrderRepository : IRepository<Order>
{
    Task<List<OrderDto>> GetOrders(Guid userId, int pageNum, int pageSize, string url);
    Task<ICollection<OrderDto>> GetOrders(int page, int lenght,string url);
    Task<Order?> GetOrder(Guid id);
    Task<Order?> GetOrder(Guid id, Guid userId);
    Task<int> GetOrders();

    Task<bool> IsExist(Guid id);
    Task<bool> IsCanCancelOrder(Guid id);
    Task<bool> IsValidTotalPrice(decimal totalPrice, ICollection<CreateOrderItemDto> items, string symbol);
    //delivery
    Task<ICollection<OrderDto>> GetOrderNoBelongToAnyDelivery(int pageNum, int pageSize,string url);
    Task<ICollection<OrderDto>> GetOrderBelongToDelivery(Guid deliveryId, int pageNum, int pageSize,string url);
    void RemoveOrderFromDelivery(Guid id, Guid deliveryId);
    Task<bool> IsSavedDistanceToOrder(Guid id);
    Task Delete(Guid id);
    void Delete(ICollection<Order> orders);
}