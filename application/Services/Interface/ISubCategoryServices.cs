using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services.Interface;

public interface ISubCategoryServices
{
    Task<IActionResult> CreateSubCategory(Guid storeId, CreateSubCategoryDto subCategoryDto);
    Task<IActionResult> UpdateSubCategory(Guid storeId, UpdateSubCategoryDto subCategoryDto);

    Task<IActionResult> DeleteSubCategory(Guid id, Guid storeId);

    Task<IActionResult> GetSubCategories(Guid storeId, int page, int length);
    Task<IActionResult> GetSubCategoryAll(Guid adminId, int page, int length);
}