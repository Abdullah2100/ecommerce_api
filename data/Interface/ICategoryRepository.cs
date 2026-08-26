using api.domain.entity;

namespace data.Interface;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetCategory(Guid id);

    Task<ICollection<Category>> GetCategories(int page, int length);
    Task<int> GetCategoriesCount();
    Task<ICollection<Category>> GetCategories(int randomNumber);

    Task<bool> IsExist(Guid id);
    Task<bool> IsExist(string name);
    Task<bool> IsExist(string name, Guid id);
    void Delete(Guid id);
    void Delete(ICollection<Category> categories);
}