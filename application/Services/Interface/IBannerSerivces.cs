using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services.Interface;

public interface IBannerServices
{
    Task<IActionResult> CreateBanner(Guid userId, CreateBannerDto bannerDto);
    Task<IActionResult> DeleteBanner(Guid id, Guid userId);

    Task<IActionResult> GetBannersAll(Guid adminId, int pageNumber, int pageSize);
    Task<IActionResult> GetBanners(Guid storeId, int pageNumber, int pageSize);
    Task<IActionResult> GetBanners(int randomLenght);
}