using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface ISubCategoryServices
{
    Task<Result<SubCategoryDto?>> CreateSubCategory(Guid storeId,CreateSubCategoryDto subCategoryDto);
    Task<Result<SubCategoryDto?>> UpdateSubCategory(Guid storeId,UpdateSubCategoryDto subCategoryDto);
    
    Task<Result<bool>> DeleteSubCategory(Guid id,Guid storeId);
    
    Task<Result<List<SubCategoryDto>>> GetSubCategories(Guid id, int page, int length);
    Task<Result<List<SubCategoryDto>>> GetSubCategoryAll(Guid adminId, int page, int length);
}