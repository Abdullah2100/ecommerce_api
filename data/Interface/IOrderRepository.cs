using api.domain.entity;
using data.dto.Request;

namespace data.Interface;

public interface IOrderRepository : IRepository<Order>
{
    Task<ICollection<Order>> GetOrders(Guid userId, int pageNum, int pageSize);
    Task<ICollection<Order>> GetOrders(int page, int lenght);
    Task<ICollection<Order>> GetOrders(int randomNumber);

    Task<Order?> GetOrder(Guid id);

    Task<Order?> GetOrder(Guid id, Guid userId);
    Task<int> GetOrders();

    Task<bool> IsExist(Guid id);
    Task<bool> IsCanCancelOrder(Guid id);
    Task<bool> IsValidTotalPrice(decimal totalPrice, ICollection<CreateOrderItemDto> items, string symbol);

    //delivery
    Task<ICollection<Order>> GetOrderNoBelongToAnyDelivery(int pageNum, int pageSize);
    Task<ICollection<Order>> GetOrderBelongToDelivery(Guid deliveryId, int pageNum, int pageSize);
    void RemoveOrderFromDelivery(Guid id, Guid deliveryId);
    Task<bool> IsSavedDistanceToOrder(Guid id);
    void Delete(Guid id);
    void Delete(ICollection<Order> orders);
}