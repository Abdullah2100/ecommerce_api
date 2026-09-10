using api.domain.entity;
using data.dto.Response;
using data.Entity;

namespace data.Interface;

public interface IStoreRepository : IRepository<Store>
{
    Task<Store?> GetStore(Guid id);
    Task<Store?> GetStoreByUserId(Guid id);
    Task<ICollection<StoreDto>> GetStores(int page, int length, string url);
    Task<List<StoreDto>> GetStores(string prefix, int length, string url);

    Task<int> GetStoresCount(int storePerPage);
    Task<bool> IsExist(string name);
    Task<bool> IsExist(string name, Guid id);
    Task<bool> IsExist(Guid id);
    Task<bool> IsExist(Guid id, Guid subCategoryId);
    void Delete(Guid id);
}