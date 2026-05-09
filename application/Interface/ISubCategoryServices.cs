using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface ISubCategoryServices
{
    Task<SubCategoryDto?> CreateSubCategory(Guid storeId,CreateSubCategoryDto subCategoryDto);
    Task<SubCategoryDto?> UpdateSubCategory(Guid storeId,UpdateSubCategoryDto subCategoryDto);
    
    Task<bool> DeleteSubCategory(Guid id,Guid storeId);
    
    Task<List<SubCategoryDto>> GetSubCategories(Guid id, int page, int length);
    Task<List<SubCategoryDto>> GetSubCategoryAll(Guid adminId, int page, int length);
}