using api.application.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using api.shared.mapper;
using api.shared.signalr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace api.application.Services;

public class OrderItemServices(
    IConfig config,
    IHubContext<OrderItemHub> hubContext,
    IUnitOfWork unitOfWork,
    IOrderServices orderServices
)
    : IOrderItemServices
{
    public async Task<IActionResult> GetOrderItems(
        Guid storeId,
        int pageNum,
        int pageSize)
    {
        var user = await unitOfWork.UserRepository.GetUser(storeId);

        var validationResult = user.IsValidateFunc(isAdmin: false, isStore: true);
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var orderItems = (await unitOfWork.OrderItemRepository
                .GetOrderItems(storeId: user!.Store!.Id, pageNum: pageNum, pageSize: pageSize))
            .Select(p => p.ToOrderItemDto(config.GetKey("url_file")))
            .ToList();

        return new ObjectResult(orderItems)
            { StatusCode = StatusCodes.Status200OK };
    }

    public async Task<IActionResult> UpdateOrderItemsStatus(
        Guid userId,
        UpdateOrderItemStatusDto orderItemsStatusDto)
    {
        var orderItem = await unitOfWork.OrderItemRepository.GetOrderItem(orderItemsStatusDto.Id);

        if (orderItem is null)
        {
            return new ObjectResult("OrderItem not Found")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        ;

        orderItem.Status = orderItemsStatusDto.Status == EnOrderItemStatusDto.Excepted
            ? EnOrderItemStatus.Excepted
            : orderItemsStatusDto.Status == EnOrderItemStatusDto.TookByDelivery
                ? EnOrderItemStatus.ReceivedByDelivery
                : EnOrderItemStatus.Cancelled;

        unitOfWork.OrderItemRepository.Update(orderItem);

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while update orderItem status")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var statusEvent = new OrderItemsStatusEvent
        {
            OrderId = orderItem.OrderId,
            OrderItemId = orderItem.Id,
            Status = orderItem.Status.ToString()
        };
        await hubContext.Clients.All.SendAsync("orderItemsStatusChange", statusEvent);


        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }
}