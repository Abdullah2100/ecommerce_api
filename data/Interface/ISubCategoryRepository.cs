using api.domain.entity;

namespace data.Interface;

public interface ISubCategoryRepository : IRepository<SubCategory>
{
    Task<SubCategory?> GetSubCategory(Guid id);
    Task<ICollection<SubCategory>> GetSubCategories(Guid storeId, int pageNumber, int pageSize);
    Task<ICollection<SubCategory>> GetSubCategories(int pageNumber, int pageSize);
    Task<int> GetSubCategoriesCount(Guid storeId);

    Task<bool> IsExist(Guid id);
    Task<bool> IsExist(Guid storeId, string name);
    Task<bool> IsExist(Guid storeId, Guid id);
    Task Delete(Guid id);
}