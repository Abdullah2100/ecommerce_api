using api.domain.entity;
using api.Presentation.dto;

namespace api.shared.mapper;

public static class OrderItemMapperExtension
{
    extension(OrderItem item)
    {
        public OrderItemDto ToOrderItemDto(string url)
        {
            return new OrderItemDto
            {
                Id = item.Id,
                OrderId = item.OrderId,
                OrderItemStatus = item.Status.ToString(),
                Price = item.Price,
                Product = item.ToOrderProductDto(url),
                Quantity = item?.Quantity ?? 0,
                OrderStatusName = item.Order.Status.ToOrderStatusName(),
                Address = item.Store?.Addresses==null ||item.Store.Addresses.Count<1? new List<AddressWithTitleDto>():
                    item.Store?.Addresses.Select(ad=>
                        new AddressWithTitleDto() 
                        {
                            Longitude = ad?.Longitude,
                            Latitude = ad?.Latitude,
                            Title = ad?.Title??"",
                        }
                    ).ToList(),
                ProductVariant =item.OrderProductsVariants==null||item.OrderProductsVariants.Count<1?new List<OrderVariantDto>():
                
                    item
                        ?.OrderProductsVariants
                        ?.Select(ost => ost.ToOrderVariantDto())
                        .ToList()
            };
        }

        public DeliveryOrderItemDto ToDeliveryOrderItemDto(string url)
        {
            return new DeliveryOrderItemDto 
            {
                Id = item.Id,
                OrderId = item.OrderId,
                OrderItemStatus = item.Status.ToString(),
                Price = item.Price,
                Product = item.ToOrderProductDto(url),
                Quantity = item?.Quantity ?? 0,
                ProductVariant = item
                    ?.OrderProductsVariants
                    ?.Select(ost => ost.ToOrderVariantDto())
                    .ToList(),
                Address = null,
            };
        }
    }
}