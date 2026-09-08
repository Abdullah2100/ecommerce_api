using api.domain.entity;
using data.dto.Response;

namespace data.Interface;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetProduct(Guid id);
    Task<Product?> GetProduct(Guid id, Guid storeId);
    Task<int> GetProduct();
    Task<int?> GetProductPages();
    Task<Product?> GetProductByUser(Guid id, Guid userId);

    Task<ICollection<ProductDto>> GetProducts(Guid storeId, Guid subCategoryId, int pageNum, int pageSize,string url);
    Task<ICollection<ProductDto>> GetProducts(Guid storeId, int pageNum, int pageSize,string url);
    Task<List<ProductDto>> GetProducts(int page, int length, string url);
    Task<ICollection<ProductDto>> GetProducts(int randomNumber,string url);
    Task<ICollection<ProductDto>> GetProductsByCategory(Guid categoryId, int pageNum, int pageSize,string url);

    Task<bool> IsExist(Guid id);
    void Delete(Guid id);
    void Delete(ICollection<Product> products);
}