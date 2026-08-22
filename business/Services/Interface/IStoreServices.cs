using api.application;
using data.dto.Request;
using data.dto.Response;

namespace business.Services.Interface;

public interface IStoreServices
{
    public Task<Result> CreateStore(CreateStoreDto store, Guid userId,string rootPath);
    public Task<Result> UpdateStore(UpdateStoreDto storeDto, Guid userId,string rootPath);
    public Task<Result> GetStoreByUserId(Guid userId);
    public Task<Result> GetStorePage(Guid adminId, int storePerPage);
    public Task<Result> GetStoreByStoreId(Guid id);
    public Task<Result> GetStores(Guid adminId, int pageNumber, int pageSize);
    public Task<Result> GetStores(Guid adminId, string prefix, int pageSize);
    public Task<Result> UpdateStoreStatus(Guid adminId, Guid storeId,Action<StoreStatusDto>sendMessage);
}