using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface IOrderItemServices
{
    
    Task<Result<List<OrderItemDto>>> GetOrderItmes(Guid storeId, int pageNum, int pageSize);
    
    Task<Result<int>> UpdateOrderItmesStatus(Guid userId, UpdateOrderItemStatusDto orderItemsStatusDto );
    

}