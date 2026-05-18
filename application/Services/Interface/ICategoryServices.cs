using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services.Interface;

public interface ICategoryServices
{
    Task<IActionResult> CreateCategory(CreateCategoryDto categoryDto, Guid adminId);
    Task<IActionResult> UpdateCategory(UpdateCategoryDto categoryDto, Guid adminId);
    Task<IActionResult> DeleteCategory(Guid categoryId, Guid adminId);
    Task<IActionResult> GetCategories(int pageNumber, int pageSize);
}