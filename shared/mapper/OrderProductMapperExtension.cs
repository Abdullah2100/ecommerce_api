using api.domain.entity;
using api.Presentation.dto;

namespace api.shared.mapper;

public static class OrderProductMapperExtension
{
    extension(OrderItem orderItem)
    {
        public OrderProductDto ToOrderProductDto(string url)
        {
            return new OrderProductDto
            {
                Id = orderItem.ProductId,
                Name = orderItem.Product.Name,
                Thumbnail=url+orderItem.Product.Thumbnail

            };
        }
    }
}