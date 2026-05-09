using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface ICategoryServices
{
    Task<CategoryDto?> CreateCategory(CreateCategoryDto categoryDto, Guid adminId);
    Task<CategoryDto?> UpdateCategory(UpdateCategoryDto categoryDto, Guid adminId);
    Task<bool> DeleteCategory(Guid categoryId, Guid adminId);
    Task<List<CategoryDto>> GetCategories(int pageNumber, int pageSize);
}