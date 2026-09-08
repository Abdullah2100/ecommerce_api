using api.domain.entity;
using data.dto.Response;

namespace data.Interface;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetCategory(Guid id);

    Task<ICollection<CategoryDto>> GetCategories(int page, int length,string url);
    Task<int> GetCategoriesCount();
    Task<ICollection<CategoryDto>> GetCategories(int randomNumber, string url);

    Task<bool> IsExist(Guid id);
    Task<bool> IsExist(string name);
    Task<bool> IsExist(string name, Guid id);
    void Delete(Guid id);
    void Delete(ICollection<Category> categories);
}