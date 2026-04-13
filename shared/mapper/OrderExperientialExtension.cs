using api.domain.entity;
using api.Presentation.dto;
using api.Presentation.dto.Response;

namespace api.shared.mapper;

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