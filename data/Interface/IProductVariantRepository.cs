using api.domain.entity;
using data.dto.Request;

namespace data.Interface;

public interface IProductVariantRepository : IRepository<ProductVariant>
{
    public Task<ProductVariant?> GetProductVarient(Guid productId, Guid id);
    Task SaveProductVariants(ICollection<ProductVariant> productVariants);
    void DeleteProductVariantByProductId(Guid productId);
    void DeleteProductVariant(List<CreateProductVariantDto> productVariants, Guid productId);
}