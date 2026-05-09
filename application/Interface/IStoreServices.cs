using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface IStoreServices
{
   public Task<StoreDto?> CreateStore(CreateStoreDto store, Guid userId);
   public Task<StoreDto?> UpdateStore(UpdateStoreDto storeDto, Guid userId);
   public Task<StoreDto?> GetStoreByUserId(Guid userId);
   public Task<int?> GetStorePage(Guid adminId, int storePerPage);
   public Task<StoreDto?> GetStoreByStoreId(Guid id);
   public Task<List<StoreDto>?> GetStores(Guid adminId, int pageNumber, int pageSize);
   public Task<List<StoreDto>?> GetStores(Guid adminId, string prefix, int pageSize);

   public Task<bool?> UpdateStoreStatus(Guid adminId, Guid storeId);
}