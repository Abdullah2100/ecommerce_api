using api.domain.entity;
using api.Presentation.dto;

namespace api.shared.mapper;

public static class ProductVariantExperientialExtension
{
    extension(ProductVariant productVariant)
    {
        public ProductVariantDto ToProductVariantDto()
        {
            return new ProductVariantDto
            {
                ProductId = productVariant.ProductId,
                Name = productVariant.Name,
                Percentage = productVariant.Percentage,
                VariantId = productVariant.VariantId,
                Id = productVariant.Id
            };
        }

        public AdminProductVariantDto ToAdminProductVariantDto()
        {
            return new AdminProductVariantDto()
            {
                Name = productVariant.Name,
                Percentage = productVariant.Percentage,
                VariantName = productVariant?.Variant?.Name??""
            
            };
        }
    }
}