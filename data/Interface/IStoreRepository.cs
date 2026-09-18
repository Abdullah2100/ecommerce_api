using api.domain.entity;
using data.dto.Response;
using data.Entity;

namespace data.Interface;

public interface IStoreRepository : IRepository<Store>
{
    Task<Store?> GetStore(Guid id, bool isTracking = true);
    Task<Store?> GetStoreByUserId(Guid id);
    Task<ICollection<StoreDto>> GetStores(int page, int length, string url);
    Task<List<StoreDto>> GetStores(string prefix, int length, string url);
    Task<int> GetStoresCount(int storePerPage);
    Task<bool> IsExist(string name);
    Task<bool> IsExist(string name, Guid id);
    void Delete(Guid id);
}