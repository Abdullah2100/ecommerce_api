using api.domain.entity;
using data.dto.Request;
using data.Entity;

namespace data.Interface;

public interface IProductVariantRepository : IRepository<ProductVariant>
{
    public Task<ProductVariant?> GetProductVariant(Guid productId, Guid id);
    Task SaveProductVariants(ICollection<ProductVariant> productVariants,bool isCreate);
    Task DeleteProductVariant(ICollection<CreateProductVariantDto> productVariants, Guid productId);
}