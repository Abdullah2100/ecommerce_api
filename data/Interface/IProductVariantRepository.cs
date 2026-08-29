using api.domain.entity;
using data.dto.Request;

namespace data.Interface;

public interface IProductVariantRepository : IRepository<ProductVariant>
{
    public Task<ProductVariant?> GetProductVariant(Guid productId, Guid id);
    Task SaveProductVariants(ICollection<ProductVariant> productVariants);
    void DeleteProductVariantByProductId(Guid productId);
    void DeleteProductVariant(ICollection<CreateProductVariantDto> productVariants, Guid productId);
}