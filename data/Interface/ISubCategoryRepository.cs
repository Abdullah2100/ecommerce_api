using api.domain.entity;
using data.dto.Response;

namespace data.Interface;

public interface ISubCategoryRepository : IRepository<SubCategory>
{
    Task<SubCategory?> GetSubCategory(Guid id);
    Task<ICollection<SubCategoryDto>> GetSubCategories(Guid storeId, int pageNumber, int pageSize);
    Task<ICollection<SubCategoryDto>> GetSubCategories(int pageNumber, int pageSize);
    Task<int> GetSubCategoriesCount(Guid storeId);

    Task<bool> IsExist(Guid id);
    Task<bool> IsExist(Guid storeId, string name);
    Task<bool> IsExist(Guid storeId, Guid id);
    Task Delete(Guid id);
}