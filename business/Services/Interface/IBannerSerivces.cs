using api.application;
using data.dto.Request;

namespace business.Services.Interface;

public interface IBannerServices
{
    Task<Result> CreateBanner(Guid userId, CreateBannerDto bannerDto,string rootPath,Action<int> sendSocketMessage);
    Task<Result> DeleteBanner(Guid id, Guid userId,string rootPath,Action<Guid> sendSocketMessage);

    Task<Result> GetBannersAll(Guid adminId, int pageNumber, int pageSize);
    Task<Result> GetBanners(Guid userId, int pageNumber, int pageSize);
    Task<Result> GetBanners(int randomLenght);
}