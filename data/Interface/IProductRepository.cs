using api.domain.entity;

namespace data.Interface;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetProduct(Guid id);
    Task<Product?> GetProduct(Guid id, Guid storeId);
    Task<int> GetProduct();
    Task<int?> GetProductPages();
    Task<Product?> GetProductByUser(Guid id, Guid userId);

    Task<ICollection<Product>> GetProducts(Guid storeId, Guid subCategoryId, int pageNum, int pageSize);
    Task<ICollection<Product>> GetProducts(Guid storeId, int pageNum, int pageSize);
    Task<ICollection<Product>> GetProducts(int page, int length);
    Task<ICollection<Product>> GetProducts(int randomNumber);
    Task<ICollection<Product>> GetProductsByCategory(Guid categoryId, int pageNum, int pageSize);

    Task<bool> IsExist(Guid id);
    void Delete(Guid id);
    void Delete(ICollection<Product> products);
}