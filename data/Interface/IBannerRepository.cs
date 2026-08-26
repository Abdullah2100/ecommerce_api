using api.domain.entity;

namespace data.Interface;

public interface IBannerRepository : IRepository<Banner>
{
    Task<Banner?> GetBanner(Guid id);
    Task<Banner?> GetBanner(Guid id, Guid storeId);

    Task<ICollection<Banner>> GetBanners(Guid id, int pageNumber, int pageSize);
    Task<ICollection<Banner>> GetBanners(int pageNumber, int pageSize);
    Task<ICollection<Banner>> GetBanners(int randomLenght);
    Task<ICollection<Banner>> GetNotActiveBanners(int randomLenght);
    Task<int> GetBannerCount();
    Task<int> GetBannerCount(Guid storeId);


    void Delete(Guid id);

    void Delete(ICollection<Banner> banners);
}