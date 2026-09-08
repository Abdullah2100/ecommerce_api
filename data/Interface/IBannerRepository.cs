using api.domain.entity;
using data.dto.Response;

namespace data.Interface;

public interface IBannerRepository : IRepository<Banner>
{
    Task<Banner?> GetBanner(Guid id);
    Task<Banner?> GetBanner(Guid id, Guid storeId);

    Task<ICollection<BannerDto>> GetBannersByStoreId(Guid id, int pageNumber, int pageSize, string url);
    Task<ICollection<BannerDto>> GetBanners( int pageNumber, int pageSize, string url);
    Task<List<BannerDto>> GetBanners(int randomLenght, string url);
    Task<ICollection<BannerDto>> GetNotActiveBanners(int randomLenght,string url);
    Task<int> GetBannerCount();
    Task<int> GetBannerCount(Guid storeId);


    void Delete(Guid id);

    void Delete(ICollection<Banner> banners);
}