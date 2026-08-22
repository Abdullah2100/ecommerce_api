using api.application;
using data.dto.Request;

namespace business.Services.Interface;

public interface ICategoryServices
{
    Task<Result> CreateCategory(CreateCategoryDto categoryDto, Guid adminId,string contentRoot);
    Task<Result> UpdateCategory(UpdateCategoryDto categoryDto, Guid adminId ,string contentRoot);
    Task<Result> DeleteCategory(Guid categoryId, Guid adminId , string contentRoot);
    Task<Result> GetCategories(int pageNumber, int pageSize);
}