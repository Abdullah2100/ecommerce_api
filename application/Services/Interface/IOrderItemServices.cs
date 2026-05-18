using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services.Interface;

public interface IOrderItemServices
{
    Task<IActionResult> GetOrderItems(Guid storeId, int pageNum, int pageSize);

    Task<IActionResult> UpdateOrderItemsStatus(Guid userId, UpdateOrderItemStatusDto orderItemsStatusDto);
}