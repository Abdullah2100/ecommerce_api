using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Interface;

public interface IStoreServices
{
    public Task<IActionResult> CreateStore(CreateStoreDto store, Guid userId);
    public Task<IActionResult> UpdateStore(UpdateStoreDto storeDto, Guid userId);
    public Task<IActionResult> GetStoreByUserId(Guid userId);
    public Task<IActionResult> GetStorePage(Guid adminId, int storePerPage);
    public Task<IActionResult> GetStoreByStoreId(Guid id);
    public Task<IActionResult> GetStores(Guid adminId, int pageNumber, int pageSize);
    public Task<IActionResult> GetStores(Guid adminId, string prefix, int pageSize);

    public Task<IActionResult> UpdateStoreStatus(Guid adminId, Guid storeId);
}