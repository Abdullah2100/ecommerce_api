using api.domain.entity;
using api.Presentation.dto;
using api.Presentation.dto.Request;

namespace api.domain.Interface;

public interface IProductVariantRepository:IRepository<ProductVariant>
{
    public Task<ProductVariant?> GetProductVarient(Guid productId,Guid id);
    Task SaveProductVariants(ICollection<ProductVariant> productVariants);
    void DeleteProductVariantByProductId(Guid productId);
    void DeleteProductVariant(List<CreateProductVariantDto> productVariants, Guid productId);
}
