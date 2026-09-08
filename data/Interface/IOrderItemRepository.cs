using api.domain.entity;
using data.dto.Response;

namespace data.Interface;

public interface IOrderItemRepository : IRepository<OrderItem>
{
    Task<ICollection<OrderItemDto>> GetOrderItems(Guid storeId, int pageNum, int pageSize,string url);
    Task<OrderItem?> GetOrderItem(Guid id, Guid storeId);
    Task<OrderItem?> GetOrderItem(Guid id);
}