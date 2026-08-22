using api.application;
using data.dto.Request;

namespace api.application.Services.Interface;

public interface ISubCategoryServices
{
    Task<Result> CreateSubCategory(Guid storeId, CreateSubCategoryDto subCategoryDto);
    Task<Result> UpdateSubCategory(Guid storeId, UpdateSubCategoryDto subCategoryDto);
    Task<Result> DeleteSubCategory(Guid id, Guid storeId);
    Task<Result> GetSubCategories(Guid storeId, int page, int length);
    Task<Result> GetSubCategoryAll(Guid adminId, int page, int length);
}