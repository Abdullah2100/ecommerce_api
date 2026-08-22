using api.application;
using data.dto.Request;
using data.dto.Response;

namespace business.Services.Interface;

public interface IOrderItemServices
{
    Task<Result> GetOrderItems(Guid storeId, int pageNum, int pageSize);

    Task<Result> UpdateOrderItemsStatus(Guid userId, UpdateOrderItemStatusDto orderItemsStatusDto,
        Action<OrderItemsStatusEvent> sendMessage);
}