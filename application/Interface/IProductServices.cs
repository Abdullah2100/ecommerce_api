using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface IProductServices
{
    Task<List<ProductDto>> GetProductsByStoreId(Guid storeId,int pageNum,int pageSize);
    Task<List<ProductDto>> GetProductsByCategoryId(Guid categryId,int pageNum,int pageSize);
    Task<List<ProductDto>> GetProducts(Guid storeId,Guid subCategoryId,int pageNum,int pageSize);
    Task<List<ProductDto>> GetProducts(int pageNum,int pageSize);
    Task<List<AdminProductsDto>> GetProductsForAdmin(Guid adminId, int pageNum, int pageSize);
    
    Task<int> GetProductsPagesForAdmin(Guid adminId,int lenght);
    Task<ProductDto?>CreateProducts(Guid userId,CreateProductDto productDto);
    Task<ProductDto?> UpdateProducts(Guid userId,UpdateProductDto productDto);
    Task<bool> DeleteProducts(Guid userId,Guid storeId,Guid id);
    
}