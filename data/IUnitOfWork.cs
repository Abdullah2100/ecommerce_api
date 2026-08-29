using data.Interface;
using data.Interface;

namespace api.Infrastructure;

public interface IUnitOfWork : IDisposable
{
    public IAddressRepository AddressRepository { get; }
    public IBannerRepository BannerRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IDeliveryRepository DeliveryRepository { get; }
    public IGeneralSettingRepository GeneralSettingRepository { get; }
    public IOrderItemRepository OrderItemRepository { get; }
    public IOrderRepository OrderRepository { get; }
    public IProductRepository ProductRepository { get; }
    public IProductImageRepository ProductImageRepository { get; }
    public IProductVariantRepository ProductVariantRepository { get; }
    public IRecreatePasswordRepository PasswordRepository { get; }
    public IStoreRepository StoreRepository { get; }
    public ISubCategoryRepository SubCategoryRepository { get; }
    public IUserRepository UserRepository { get; }
    public IVariantRepository VariantRepository { get; }
    public IOrderProductVariant OrderProductVariantRepository { get; }
    public IAnalyseRepository AnalyseRepository { get; }
    public ICurrencyRepository CurrencyRepository { get; }
    public IPaymentTypeRepository PaymentTypeRepository { get; set; }
    public IUserRefreshTokenRepository UserRefreshTokenRepository { get; set; }


    public Task<int> SaveChanges();
}