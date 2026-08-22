using api.application;
using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using business.mapper;
using api.util;
using business.Services.Interface;
using data.dto.Request;
using data.dto.Response;
using data.util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace business.Services.Implement;

public class OrderServices(
    IConfiguration config,
    IUnitOfWork unitOfWork,
    IServiceProvider sp,
    //  
    HybridCache cache,
    ILogger<OrderServices> logger)
    : IOrderServices
{
    private static readonly List<string> OrderStatus = new List<string>
        { "Rejected", "Inprogress", "Accepted", "In away", "Received", "Completed" };

    public async Task<Result> CreateOrder(Guid userId, CreateOrderDto orderDto, Action<OrderDto> sendMessage)
    {
        logger.LogInformation("start create order");

        var user = await unitOfWork.UserRepository.GetUser(userId);
        var validationResult = user.IsValidateFunc(isAdmin: false);

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", user?.Id, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        if (!await unitOfWork.OrderRepository.IsValidTotalPrice(orderDto.TotalPrice, orderDto.Items, orderDto.Symbol))
        {
            logger.LogError("not valid total price  totalPrice {totalPrice } for order   from user {userId}",
                orderDto.TotalPrice, user?.Id);
            return new Result(false, "order totalPrice is not valid", null, 409);
        }

        var paymentType = (await unitOfWork.PaymentTypeRepository.GetPaymentTypeGetPayment(orderDto.PaymentTypeId));

        if (paymentType is null)
        {
            logger.LogError("not found paymentType {paymentTypeId}", orderDto.paymentId);
            return new Result(false, "payment type is not exist ", null, 404);
        }

        if (paymentType?.Name?.ToLower() != "Cash")
        {
            var stripPayment = new PaymentServices(new StripPaymentServices());
            var isPassed = await stripPayment.IsValidatePayment(orderDto.PaymentId ?? "");
            if (!isPassed)
            {
                logger.LogError("payment for order was feild from {paymentTypeName}", paymentType?.Name);
                return new Result(false, "payment  is not successfully", null, 404);
            }
        }

        var id = ClsUtil.GenerateGuid();
        var order = new Order
        {
            Id = id,
            PaymentTypeId = orderDto.PaymentTypeId,
            Longitude = orderDto.Longitude,
            Latitude = orderDto.Latitude,
            UserId = userId,
            TotalPrice = orderDto.TotalPrice,
            Status = 1,
            CreatedAt = DateTime.Now,
            UpdatedAt = null,
            Symbol = orderDto.Symbol
        };

        unitOfWork.OrderRepository.Add(order);

        foreach (var item in orderDto.Items)
        {
            var orderItemId = ClsUtil.GenerateGuid();
            var orderProductsVariants = item.ProductVariant == null || item.ProductVariant.Count == 0
                ? null
                : item.ProductVariant.Select(x => new OrderProductsVariant
                {
                    Id = ClsUtil.GenerateGuid(),
                    OrderItemId = orderItemId,
                    ProductVariantId = x
                }).ToList();

            var orderItem = new OrderItem
            {
                Id = orderItemId,
                OrderId = id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                StoreId = item.StoreId,
                Price = item.Price,
            };
            unitOfWork.OrderItemRepository.Add(orderItem);

            if (orderProductsVariants is not null)
                unitOfWork.OrderProductVariantRepository.Add(orderProductsVariants);
        }

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("could not submit order to db");
            return new Result(false, "error while create order", null, 500);
        }

        var isSavedDistance = await unitOfWork.OrderRepository.IsSavedDistanceToOrder(order.Id);
        await unitOfWork.SaveChanges();

        if (!isSavedDistance)
        {
            logger.LogError("could not submit distanc calculation to order {orderId}", order.Id);
            return new Result(false, "could not calculate  distance distance to user ", null, 500);
        }

        order = await unitOfWork.OrderRepository.GetOrder(order.Id);
        if (order is null)
        {
            logger.LogError("could not order by {Id}", order?.Id);
            return new Result(false, "error while create order", null, 500);
        }

        var dtoOrder = order.ToDto(config["url_file"] ?? "");

        sendMessage.Invoke(dtoOrder);
        await SendNotification(order, 1);
        await cache.RemoveByTagAsync(MemoryCacheKeys.OrdersKey);

        logger.LogInformation("end create order");
        return new Result(true, null, dtoOrder, 201);
    }

    public async Task<Result> GetMyOrders(Guid userId, int pageNum, int pageSize)
    {
        logger.LogInformation("start getting order page by page by userId");

        var orders = await cache.GetOrCreateAsync(MemoryCacheKeys.OrdersKey + "/" + userId + "/" + pageNum, async ct =>
            {
                var orders = (await unitOfWork.OrderRepository.GetOrders(userId, pageNum, pageSize))
                    .Select(o => o.ToDto(config["url_file"] ?? ""))
                    .ToList();
                return orders;
            },
            tags: [MemoryCacheKeys.OrdersKey]);

        logger.LogInformation("end getting order page by page by userId");
        return new Result(true, null, orders, 200);
    }

    //for admin dashboard
    public async Task<Result> GetOrders(Guid userId, int pageNum, int pageSize)
    {
        logger.LogInformation("start getting order page by page for dashboard");

        var delivery = await unitOfWork.UserRepository.GetUser(userId);
        var validationResult = delivery.IsValidateFunc();

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", userId, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var orders = await cache.GetOrCreateAsync(MemoryCacheKeys.OrdersKey + "/dashbord" + userId + "/" + pageNum,
            async ct =>
            {
                var orders = (await unitOfWork.OrderRepository.GetOrders(pageNum, pageSize))
                    .Select(o => o.ToDto(config["url_file"] ?? ""))
                    .ToList();

                var orderPages = (int)Math.Ceiling((double)orders.Count / pageSize);
                var holder = new AdminOrderDto { Orders = orders, pageNum = orderPages };
                return orders;
            },
            tags: [MemoryCacheKeys.OrdersKey]);

        logger.LogInformation("end getting order page by page for dashboard");
        return new Result(true, null, orders, 200);
    }

    public async Task<Result> UpdateOrderStatus(Guid id, int status, Action<UpdateOrderStatusEventDto> sendMessage)
    {
        logger.LogInformation("start updating order status");

        var order = await unitOfWork.OrderRepository.GetOrder(id);

        if (order is null)
        {
            logger.LogError("not found order by {orderId}", id);
            return new Result(false, "order not found", null, 404);
        }

        order.Status = status;

        unitOfWork.OrderRepository.Update(order);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("error while updating  order status  by {orderId} to {status}", id, OrderStatus[status]);
            return new Result(false, "error while update order status", null, 500);
        }

        var orderStatus = new UpdateOrderStatusEventDto
        {
            Id = order.Id,
            Status = OrderStatus[status]
        };

        sendMessage.Invoke(orderStatus);

        await SendNotification(order, status);
        await cache.RemoveByTagAsync(MemoryCacheKeys.OrdersKey);

        logger.LogInformation("end updating order status");
        return new Result(true, null, null, 204);
    }

    public async Task<Result> DeleteOrder(Guid id, Guid userId)
    {
        logger.LogInformation("start deleting order");

        var order = await unitOfWork.OrderRepository.GetOrder(id, userId);
        if (order is null)
        {
            logger.LogError("not found order by {orderId}", id);
            return new Result(false, "order not found", null, 404);
        }

        unitOfWork.OrderRepository.Delete(id);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("could not delete order {orderId}", id);
            return new Result(false, "error while delete order", null, 500);
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.OrdersKey);
        logger.LogInformation("end deleting order");
        return new Result(true, null, null, 204);
    }



    // for delivery 
    public async Task<Result> GetOrdersByDeliveryId(Guid deliveryId, int pageNum, int pageSize)
    {
        logger.LogInformation("start getting orders page by page by deliveryI");

        var delivery = await unitOfWork.DeliveryRepository.GetDelivery(deliveryId);
        var validationResult = delivery.IsValidated();

        if (validationResult is not null)
        {
            logger.LogError("delivery not valid {deliveryId} validationError {message}", delivery?.Id,
                validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var orders = await cache.GetOrCreateAsync(MemoryCacheKeys.OrdersKey + "/my" + delivery + "/" + pageNum,
            async ct =>
            {
                var orders = (await unitOfWork.OrderRepository.GetOrderBelongToDelivery(deliveryId, pageNum, pageSize))
                    .Select(o => o.ToDto(config["url_file"] ?? ""))
                    .ToList();
                return orders;
            },
            tags: [MemoryCacheKeys.OrdersKey]);

        logger.LogInformation("end getting orders page by page by deliveryI");
        return new Result(true, null, orders, 200);
    }

    public async Task<Result> GetOrdersNotBelongToDeliveries(Guid deliveryId, int pageNum, int pageSize)
    {
        logger.LogInformation("start getting order not belong todeliveryId page by page");
        var delivery = await unitOfWork.DeliveryRepository.GetDelivery(deliveryId);
        var validationResult = delivery.IsValidated();

        if (validationResult is not null)
        {
            logger.LogError("delivery not valid {deliveryId} validationError {message}", deliveryId,
                validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var orders = await cache.GetOrCreateAsync(
            MemoryCacheKeys.OrdersKey + "/not-belong-to" + delivery + "/" + pageNum,
            async ct =>
            {
                var orders = (await unitOfWork.OrderRepository.GetOrderNoBelongToAnyDelivery(pageNum, pageSize))
                    .Select(o => o.ToDto(config["url_file"] ?? ""))
                    .ToList();
                return orders;
            },
            tags: [MemoryCacheKeys.OrdersKey]);

        logger.LogInformation("end getting order not belong todeliveryId page by page");
        return new Result(true, null, orders, 200);
    }


    public async Task<Result> SubmitOrderToDelivery(
        Guid id,
        Guid deliveryId,
        Action<OrderTookByEvent> sendMessage1,
        Action<UpdateOrderStatusEventDto> sendMessage2)
    {
        logger.LogInformation("start submit order to delivery");

        var delivery = await unitOfWork.DeliveryRepository.GetDelivery(deliveryId);
        var validationResult = delivery.IsValidated();

        if (validationResult is not null)
        {
            logger.LogError("delivery not valid {deliveryId} validationError {message}", deliveryId,
                validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var order = await unitOfWork.OrderRepository.GetOrder(id);

        if (order == null)
        {
            logger.LogError("not found order by  {orderId}", id);
            return new Result(false, "Order not Found", null, 409);
        }

        if (order.DeliveryId != null)
        {
            logger.LogError("order  {orderId} alredy linked to {deliveryId}", id, order.DeliveryId);
            return new Result(false, "Order Delivered By another Delivery", null, 409);
        }

        order.DeliveryId = deliveryId;
        order.UpdatedAt = DateTime.Now;

        unitOfWork.OrderRepository.Update(order);
        var result = await unitOfWork.SaveChanges();


        if (result < 1)
        {
            logger.LogError("error while submit order {orderId} to deliveryId {devlieryId}", id, order.DeliveryId);
            return new Result(false, "error while update order", null, 500);
        }

        var eventHolder = new OrderTookByEvent { Id = id, DeliveryId = deliveryId };

        sendMessage1.Invoke(eventHolder);
        sendMessage2.Invoke(new UpdateOrderStatusEventDto { Id = order.Id, Status = OrderStatus[2] });
        await SendNotification(order, 2);
        await cache.RemoveByTagAsync(MemoryCacheKeys.OrdersKey);

        logger.LogInformation("end submit order to delivery");
        return new Result(true, null, null, 204);
    }


    public async Task<Result> CancelOrderFromDelivery(
        Guid id,
        Guid deliveryId,
        Action<OrderDto> sendMessage
    )
    {
        logger.LogInformation("start make order not belong to delivery status");

        var delivery = await unitOfWork.DeliveryRepository.GetDelivery(deliveryId);
        var validationResult = delivery.IsValidated();

        if (validationResult is not null)
        {
            logger.LogError("delivery not valid {deliveryId} validationError {message}", deliveryId,
                validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var order = await unitOfWork.OrderRepository.GetOrder(id);

        if (order is null)
        {
            logger.LogError("not found order by  {orderId}", id);
            return new Result(false, "order not found ", null, 404);
        }

        if (!await unitOfWork.OrderRepository.IsCanCancelOrder(id))
        {
            logger.LogError(
                "could not cancel the order {orderId} delivery {deliveryId} already catch some orderItem from Stores ",
                id, deliveryId);
            return new Result(false, "order can not cancel some order items received from stores by delivery ", null,
                403);
        }

        unitOfWork.OrderRepository.RemoveOrderFromDelivery(id, deliveryId);
        var result = await unitOfWork.SaveChanges();


        if (result == 0)
        {
            logger.LogError("error while cancel order {orderId} from deliveryId {devlieryId}", id, order.DeliveryId);
            return new Result(false, "error while remove order from delivery", null, 500);
        }

        sendMessage.Invoke(order.ToDto(config["url_file"] ?? ""));
        await cache.RemoveByTagAsync(MemoryCacheKeys.OrdersKey);

        logger.LogInformation("end make order not belong to delivery status");
        return new Result(true, null, null, 204);
    }

    public async Task<Result> GetOrdersStatus(Guid adminId)
    {
        logger.LogInformation("end make order not belong to delivery status");

        var user = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = user.IsValidateFunc();

        if (validationResult is not null)
        {
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        return new Result(true, null, OrderStatus, 200);
    }

    private async Task SendNotification(Order order, int status)
    {
        await SendNotificationToStore(order, status);
        await SendNotificationToUser(order, status);
        await SendNotificationToDelivery(order, status);
    }

    private async Task SendNotificationToStore(Order order, int status)
    {
        try
        {
            var messageServe = sp.GetRequiredKeyedService<IMessageService>(EnMessageService.Notification);
            var orderItems = order.Items.ToList();

            foreach (var orderItem in orderItems)
            {
                var cancelMessage = orderItem.Product.Name + " is Rejected For " + order.User.Name;
                var storeMessage = StoreMessage(status, cancelMessage);
                if (!string.IsNullOrEmpty(storeMessage) && orderItem.Store.user.DeviceToken is not null)
                {
                    await messageServe.SendingMessage(storeMessage, orderItem.Store.user.DeviceToken);
                }
            }
        }
        catch (System.Exception e)
        {
            Console.WriteLine($"Error from notification service: {e.Message}");
        }
    }

    private async Task SendNotificationToUser(Order order, int status)
    {
        try
        {
            var messageServe = sp.GetRequiredKeyedService<IMessageService>(EnMessageService.Notification);
            var customerMessage = UserMessage(status);
            if (!string.IsNullOrEmpty(customerMessage) && order.User.DeviceToken is not null)
            {
                await messageServe.SendingMessage(customerMessage, order.User.DeviceToken);
            }
        }
        catch (System.Exception e)
        {
            Console.WriteLine($"Error from notification service: {e.Message}");
        }
    }
    
    
    private async Task SendNotificationToDelivery(Order order, int status)
    {
        try
        {
            var messageServe = sp.GetRequiredKeyedService<IMessageService>(EnMessageService.Notification);

            var deliveryMessage = DeliveryMessage(status);

            Delivery? delivery = null;
            if (order.DeliveryId is not null)
            {
                delivery = await unitOfWork.DeliveryRepository.GetDelivery(order.DeliveryId ?? Guid.Empty);
            }

            switch (status)
            {
                case 0:
                    {
                        await messageServe.SendingMessage(deliveryMessage, delivery?.DeviceToken ?? "");
                    }
                    break;

                case 1:
                    {
                        await SendNotificationToDeliveries(deliveryMessage, messageServe);
                    }
                    break;

                case 5:
                    {
                        await messageServe.SendingMessage(deliveryMessage, delivery?.DeviceToken ?? "");
                    }
                    break;
            }
        }
        catch
            (System.Exception e)
        {
            Console.WriteLine($"Error from notification service: {e.Message}");
        }
    }

    private async Task SendNotificationToDeliveries(string message, IMessageService messageServe)
    {
        try
        {
            var deliveriesLenght = await unitOfWork.DeliveryRepository.GetDeliveriesPage(20);
            for (int i = 0; i < deliveriesLenght; i++)
            {
                var deliveryList = await unitOfWork.DeliveryRepository.GetDeliveries(i + 1, 20);
                if (deliveryList is null) continue;
                foreach (var delivery in deliveryList)
                {
                    if (delivery.DeviceToken is not null)
                        await messageServe.SendingMessage(message, delivery.DeviceToken!);
                }
            }
        }
        catch
            (System.Exception e)
        {
            Console.WriteLine($"Error from notification service: {e.Message}");
        }
    }

    private static string UserMessage(int status)
    {
        return status switch
        {
            0 => "Your Order is Rejected",
            1 => "Your Order is Submit Successful",
            2 => "Your Order is Accepted By Delivery Man",
            3 => "Your Order in Away to Your Place",
            4 => "Your Order is Received",
            5 => "Your Order is Delivered",
            _ => ""
        };
    }

    private static string DeliveryMessage(int status)
    {
        return status switch
        {
            0 => "Order is Rejected",
            1 => "New Order is Submit",
            5 => "Your Order is Received",
            _ => ""
        };
    }

    private static string StoreMessage(int status, string customMessage = "")
    {
        return status switch
        {
            0 => customMessage,
            2 => "There Are New Order For Your Store Check them",
            5 => "Your Order is Delivered",
            _ => ""
        };
    }
}