using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface IBannerSerivces
{
    Task<BannerDto?> CreateBanner(Guid userId, CreateBannerDto bannerDto);
    Task<bool> DeleteBanner(Guid id, Guid userId);

    Task<List<BannerDto>> GetBannersAll(Guid adminId, int pageNumber, int pageSize);
    Task<List<BannerDto>> GetBanners(Guid storeId, int pageNumber, int pageSize);
    Task<List<BannerDto>> GetBanners(int randomLenght);
}