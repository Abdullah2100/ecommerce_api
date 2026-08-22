using api.application;
using api.domain.entity;
using api.Infrastructure;
using business.mapper;
using api.util;
using business.Services.Interface;
using data.dto.Request;
using data.dto.Response;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace business.Services.Implement;

public class OrderItemServices(
    IConfiguration config,
    IUnitOfWork unitOfWork,
    HybridCache cache,
    ILogger<OrderItemServices> logger)
    : IOrderItemServices
{
    public async Task<Result> GetOrderItems(Guid storeId, int pageNum, int pageSize)
    {
        logger.LogInformation("start getting orderItems page by page by storeId");
        var user = await unitOfWork.UserRepository.GetUser(storeId);

        var validationResult = user.IsValidateFunc(isAdmin: false, isStore: true);
        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", user?.Id, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var orderItems = await cache.GetOrCreateAsync(
            MemoryCacheKeys.OrderItemsKey + "/" + storeId + "/" + pageNum,
            async ct =>
            {
                var orderItems = (await unitOfWork.OrderItemRepository
                        .GetOrderItems(storeId: user!.Store!.Id, pageNum: pageNum, pageSize: pageSize))
                    .Select(p => p.ToOrderItemDto(config["url_file"] ?? ""))
                    .ToList();
                return orderItems;
            },
            tags: [MemoryCacheKeys.OrderItemsKey]);

        logger.LogInformation("end getting orderItems page by page by storeId");
        return new Result(true, null, orderItems, 200);
    }

    public async Task<Result> UpdateOrderItemsStatus(Guid userId, UpdateOrderItemStatusDto orderItemsStatusDto,
        Action<OrderItemsStatusEvent> sendMessage)
    {
        logger.LogInformation("start updating orderItem Status");
        var orderItem = await unitOfWork.OrderItemRepository.GetOrderItem(orderItemsStatusDto.Id);

        if (orderItem is null)
        {
            logger.LogError("orderItem {orderId} not found ", orderItemsStatusDto.Id);
            return new Result(false, "OrderItem not Found", null, 404);
        }

        orderItem.Status = orderItemsStatusDto.Status == EnOrderItemStatusDto.Excepted
            ? EnOrderItemStatus.Excepted
            : orderItemsStatusDto.Status == EnOrderItemStatusDto.TookByDelivery
                ? EnOrderItemStatus.ReceivedByDelivery
                : EnOrderItemStatus.Cancelled;

        unitOfWork.OrderItemRepository.Update(orderItem);

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("start updating orderItem Status");
            return new Result(false, "error while update orderItem status", null, 500);
        }

        var statusEvent = new OrderItemsStatusEvent
        {
            OrderId = orderItem.OrderId,
            OrderItemId = orderItem.Id,
            Status = orderItem.Status.ToString()
        };

        logger.LogInformation(" updating orderItem successfully for {orderItmeId}", orderItem.Id);
        sendMessage.Invoke(statusEvent);
        await cache.RemoveByTagAsync(MemoryCacheKeys.OrderItemsKey);

        logger.LogInformation("end updating orderItem Status");
        return new Result(true, null, null, 204);
    }
}