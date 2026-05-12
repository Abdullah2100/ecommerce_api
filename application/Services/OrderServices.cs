using api.application.Interface;
using api.application.Result;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using api.shared.mapper;
using api.shared.signalr;
using api.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace api.application.Services;

public class OrderServices(
    IUnitOfWork unitOfWork,
    IConfig config,
    IHubContext<OrderHub> hubContext,
    IServiceProvider sp)
    : IOrderServices
{
    private static readonly List<string> OrderStatus = new List<string>
    {
        "Rejected",
        "Inprogress",
        "Accepted",
        "In away",
        "Received",
        "Completed",
    };


    public async Task<IActionResult> CreateOrder(Guid userId, CreateOrderDto orderDto)
    {
        var user = await unitOfWork.UserRepository.GetUser(userId);

        var validationResult = user.IsValidateFunc(isAdmin: false);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        if (!(await unitOfWork.OrderRepository.IsValidTotalPrice(orderDto.TotalPrice, orderDto.Items, orderDto.Symbol)))
        {
            return new ObjectResult("order totalPrice is not valid")
                { StatusCode = StatusCodes.Status409Conflict };
        }

        var paymentType =
            (await unitOfWork.PaymentTypeRepository.GetPaymentTypeGetPayment(orderDto.PaymentTypeId));

        if (paymentType is null)
        {
            return new ObjectResult("payment type is not exist ")
                { StatusCode = StatusCodes.Status404NotFound };
        }


        // to continue the  payment if it is not cash

        if (paymentType?.Name?.ToLower() != "Cash")
        {
            var stripPayment = new PaymentServices(new StripPaymentServices());
            var isPassed = await stripPayment.IsValidatePayment(orderDto.PaymentId ?? "");
            if (!isPassed)
                return new ObjectResult("payment  is not successfully")
                    { StatusCode = StatusCodes.Status404NotFound };
        }


        //  this for production is used to keep order under 40 order on vps
        var ordersCount = await unitOfWork.OrderRepository.GetOrders();

        if (ordersCount > 40)
        {
            var orders = await unitOfWork.OrderRepository.GetOrders(40);
            unitOfWork.OrderRepository.Delete(orders.ToList());
        }
        //end


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
            var orderProductsVariants =
                item.ProductVariant == null || item.ProductVariant.Count == 0
                    ? null
                    : item.ProductVariant
                        .Select(x => new OrderProductsVariant
                        {
                            Id = ClsUtil.GenerateGuid(),
                            OrderItemId = orderItemId,
                            ProductVariantId = x
                        })
                        .ToList();

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
            return new ObjectResult("error while create order")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var isSavedDistance = await unitOfWork.OrderRepository.IsSavedDistanceToOrder(order.Id);
        await unitOfWork.SaveChanges();

        if (!isSavedDistance)
        {
            return new ObjectResult("could not calculate  distance distance to user ")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        order = await unitOfWork.OrderRepository.GetOrder(order.Id);
        if (order is null)
        {
            return new ObjectResult("error while create order")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var dtoOrder = order.ToDto(config.GetKey("url_file"));
        await hubContext.Clients.All.SendAsync("createdOrder", dtoOrder);
        await SendNotification(order, 1);


        return new ObjectResult(dtoOrder)
            { StatusCode = StatusCodes.Status201Created };
    }


    public async Task<IActionResult> GetMyOrders(Guid userId, int pageNum, int pageSize)
    {
        var orders = (await unitOfWork.OrderRepository
                .GetOrders(userId, pageNum, pageSize))
            .Select(o => o.ToDto(config.GetKey("url_file")))
            .ToList();


        return new ObjectResult(orders)
            { StatusCode = StatusCodes.Status200OK };
    }

    //for admin dashboard
    public async Task<IActionResult> GetOrders(Guid userId, int pageNum, int pageSize)
    {
        var delivery = await unitOfWork.UserRepository.GetUser(userId);

        var validationResult = delivery.IsValidateFunc();

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var orders = (await unitOfWork.OrderRepository
                .GetOrders(pageNum, pageSize))
            .Select(o => o.ToDto(config.GetKey("url_file")))
            .ToList();

        var orderPages = (int)Math.Ceiling((double)orders.Count / pageSize);

        var holder = new AdminOrderDto { Orders = orders, pageNum = orderPages };

        return new ObjectResult(holder)
            { StatusCode = StatusCodes.Status200OK };
    }

    public async Task<IActionResult> UpdateOrderStatus(Guid id, int status)
    {
        var order = await unitOfWork.OrderRepository
            .GetOrder(id);

        if (order is null)
        {
            return new ObjectResult("order not found")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        order.Status = status;

        unitOfWork.OrderRepository.Update(order);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while update order status")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        await hubContext.Clients.All.SendAsync("orderStatus", new UpdateOrderStatusEventDto
        {
            Id = order.Id,
            Status = OrderStatus[status]
        });


        await SendNotification(order, status);

        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> DeleteOrder(Guid id, Guid userId)
    {
        var order = await unitOfWork.OrderRepository.GetOrder(id, userId);
        if (order is null)
        {
            return new ObjectResult("order not found")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        unitOfWork.OrderRepository.Delete(id);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while delete order")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }


        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }


    // for delivery 
    public async Task<IActionResult> GetOrdersByDeliveryId(Guid deliveryId, int pageNum, int pageSize)
    {
        var delivery = await unitOfWork.DeliveryRepository.GetDelivery(deliveryId);

        var validationResult = delivery.IsValidated();

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var orders = (await unitOfWork.OrderRepository
                .GetOrderBelongToDelivery(deliveryId, pageNum, pageSize))
            .Select(o => o.ToDto(config.GetKey("url_file")))
            .ToList();


        return new ObjectResult(orders) { StatusCode = StatusCodes.Status200OK };
    }

    public async Task<IActionResult> GetOrdersNotBelongToDeliveries(Guid deliveryId, int pageNum, int pageSize)
    {
        var delivery = await unitOfWork.DeliveryRepository.GetDelivery(deliveryId);

        var validationResult = delivery.IsValidated();
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var orders = (await unitOfWork.OrderRepository
                .GetOrderNoBelongToAnyDelivery(pageNum, pageSize))
            .Select(o => o.ToDto(config.GetKey("url_file")))
            .ToList();


        return new ObjectResult(orders)
            { StatusCode = StatusCodes.Status200OK };
    }


    public async Task<IActionResult> SubmitOrderToDelivery(Guid id, Guid deliveryId)
    {
        var delivery = await unitOfWork.DeliveryRepository.GetDelivery(deliveryId);

        var validationResult = delivery.IsValidated();

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }


        var order = await unitOfWork.OrderRepository.GetOrder(id);

        if (order == null)
        {
            return new ObjectResult("Order not Found")
                { StatusCode = StatusCodes.Status409Conflict };
        }

        if (order.DeliveryId != null)
            return new ObjectResult("Order Delivered By another Delivery")
                { StatusCode = StatusCodes.Status409Conflict };


        order.DeliveryId = deliveryId;
        order.UpdatedAt = DateTime.Now;

        unitOfWork.OrderRepository.Update(order);

        var result = await unitOfWork.SaveChanges();


        if (result < 1)
        {
            return new ObjectResult("error while update order")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var eventHolder = new OrderTookByEvent
        {
            Id = id,
            DeliveryId = deliveryId
        };

        await hubContext.Clients.All.SendAsync("orderGettingByDelivery", eventHolder);
        await hubContext.Clients.All.SendAsync("orderStatus", new UpdateOrderStatusEventDto
        {
            Id = order.Id,
            Status = OrderStatus[2]
        });


        await SendNotification(order, status: 2);

        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> CancelOrderFromDelivery(Guid id, Guid deliveryId)
    {
        var delivery = await unitOfWork.DeliveryRepository.GetDelivery(deliveryId);

        var validationResult = delivery.IsValidated();
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var order = await unitOfWork.OrderRepository.GetOrder(id);

        if (order is null)
        {
            return new ObjectResult("order not found ")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        if (!(await unitOfWork.OrderRepository.IsCanCancelOrder(id)))
        {
            return new ObjectResult("order can not cancel some order items received from stores by delivery ")
                { StatusCode = StatusCodes.Status403Forbidden };
        }

        unitOfWork.OrderRepository.RemoveOrderFromDelivery(id, deliveryId);
        var result = await unitOfWork.SaveChanges();


        if (result == 0)
        {
           
            return new ObjectResult("error while remove order from delivery")
                { StatusCode = StatusCodes.Status500InternalServerError };

        }

        await hubContext.Clients.All.SendAsync("createdOrder", order.ToDto(config.GetKey("url_file")));

       
        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };

    }

    public async Task<IActionResult> GetOrdersStatus(Guid adminId)
    {
        var user = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = user.IsValidateFunc();

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
 
        }

       
        return new ObjectResult(OrderStatus)
            { StatusCode = StatusCodes.Status200OK };

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

            var userMessage = UserMessage(status);
            if (!string.IsNullOrEmpty(userMessage))
            {
                await messageServe.SendingMessage(userMessage, order.User?.DeviceToken ?? "");
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