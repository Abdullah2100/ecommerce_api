using api.domain.entity;

namespace data.Interface;

public interface IOrderItemRepository : IRepository<OrderItem>
{
    Task<IEnumerable<OrderItem>> GetOrderItems(Guid storeId, int pageNum, int pageSize);
    Task<OrderItem?> GetOrderItem(Guid id, Guid storeId);
    Task<OrderItem?> GetOrderItem(Guid id);
}