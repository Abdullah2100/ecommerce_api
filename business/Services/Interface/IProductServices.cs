using api.application;
using data.Dto.Request;

namespace business.Services.Interface;

public interface IProductServices
{
    Task<Result> GetProductsByStoreId(Guid storeId, int pageNum, int pageSize);
    Task<Result> GetProductsByCategoryId(Guid categoryId, int pageNum, int pageSize);
    Task<Result> GetProducts(Guid storeId, Guid subCategoryId, int pageNum, int pageSize);
    Task<Result> GetProducts(int pageNum, int pageSize);
    Task<Result> GetProductsForAdmin(Guid adminId, int pageNum, int pageSize);
    Task<Result> GetProductsPagesForAdmin(Guid adminId, int length);
    Task<Result> CreateProducts(Guid userId, CreateProductDto productDto,string rootPath);
    Task<Result> UpdateProducts(Guid userId, UpdateProductDto productDto, string rootPath);
    Task<Result> DeleteProducts(Guid userId, Guid storeId, Guid id,string rootPath);
}