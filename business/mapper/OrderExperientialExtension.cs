using api.domain.entity;
using data.dto.Response;

namespace business.mapper;

public static class OrderExperientialExtension
{
    extension(OrderProductsVariant orderProductsVariant)
    {
        public OrderVariantDto ToOrderVariantDto()
        {
            return new OrderVariantDto
            {
                Name = orderProductsVariant.ProductVariant?.Product?.Name,
                VariantName = orderProductsVariant.ProductVariant?.Variant?.Name,
            };
        }
    }
}