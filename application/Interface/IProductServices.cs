using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Interface;

public interface IProductServices
{
    Task<IActionResult> GetProductsByStoreId(Guid storeId, int pageNum, int pageSize);
    Task<IActionResult> GetProductsByCategoryId(Guid categoryId, int pageNum, int pageSize);
    Task<IActionResult> GetProducts(Guid storeId, Guid subCategoryId, int pageNum, int pageSize);
    Task<IActionResult> GetProducts(int pageNum, int pageSize);
    Task<IActionResult> GetProductsForAdmin(Guid adminId, int pageNum, int pageSize);

    Task<IActionResult> GetProductsPagesForAdmin(Guid adminId, int length);
    Task<IActionResult> CreateProducts(Guid userId, CreateProductDto productDto);
    Task<IActionResult> UpdateProducts(Guid userId, UpdateProductDto productDto);
    Task<IActionResult> DeleteProducts(Guid userId, Guid storeId, Guid id);
}