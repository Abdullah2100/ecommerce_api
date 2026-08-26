using api.domain.entity;

namespace data.Interface;

public interface IProductImageRepository : IRepository<ProductImage>
{
    void DeleteProductImages(Guid id);
    void DeleteProductImages(ICollection<string> images, Guid id);
    void AddProductImage(ICollection<ProductImage> productImage);
    Task<ICollection<string>> GetProductImages(Guid id);
}