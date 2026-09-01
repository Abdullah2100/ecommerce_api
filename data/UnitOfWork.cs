using api.application;
using api.Infrastructure;
using data.Repositories;
using data.Interface;
using data.Repositories;
using Microsoft.Extensions.Logging;

namespace data;

public class UnitOfWork(
    ILogger<UnitOfWork> logger,
    ILogger<AddressRepository> addressLogger,
    ILogger<BannerRepository> bannerLogger,
    ILogger<CategoryRepository> categoryLogger,
    ILogger<DeliveryRepository> deliveryLogger,
    ILogger<GeneralSettingRepository> generalSettingLogger,
    ILogger<OrderItemRepository> orderItemLogger,//from hear
    ILogger<ProductVariantRepository> productVariantLogger,
    ILogger<ProductImageRepository> productImageLogger,
    ILogger<ProductRepository> productLogger,
    ILogger<OrderRepository> orderLogger,
    ILogger<ReseatPasswordRepository> reseatPasswordLogger,
    ILogger<StoreRepository> storeLogger,
    ILogger<SubCategoryRepository> subCategoryLogger,
    ILogger<UserRepository> userLogger,
    Logger<VariantRepository> variantLogger,
    ILogger<CurrencyRepository> currencyLogger,
    ILogger<PaymentTypeRepository> paymentTypeLogger,
    ILogger<UserRefreshTokenRepository> userRefreshmentLogger,
    AppDbContext context) : IUnitOfWork
{
    public IAddressRepository AddressRepository { get; set; } = new AddressRepository(context, addressLogger);
    public IBannerRepository BannerRepository { get; set; } = new BannerRepository(context, bannerLogger);
    public ICategoryRepository CategoryRepository { get; set; } = new CategoryRepository(context, categoryLogger);
    public IDeliveryRepository DeliveryRepository { get; set; } = new DeliveryRepository(context,deliveryLogger);
    public IGeneralSettingRepository GeneralSettingRepository { get; set; } = new GeneralSettingRepository(context,generalSettingLogger);
    public IOrderItemRepository OrderItemRepository { get; set; } = new OrderItemRepository(context,orderItemLogger);
    public IProductVariantRepository ProductVariantRepository { get; set; } = new ProductVariantRepository(context,productVariantLogger);
    public IProductImageRepository ProductImageRepository { get; set; } = new ProductImageRepository(context,productImageLogger);
    public IProductRepository ProductRepository { get; set; } = new ProductRepository(context,productLogger);
    public IOrderRepository OrderRepository { get; set; } = new OrderRepository(context,orderLogger);
    public IRecreatePasswordRepository PasswordRepository { get; set; } = new ReseatPasswordRepository(context,reseatPasswordLogger);
    public IStoreRepository StoreRepository { get; set; } = new StoreRepository(context,storeLogger);
    public ISubCategoryRepository SubCategoryRepository { get; set; } = new SubCategoryRepository(context,subCategoryLogger);
    public IUserRepository UserRepository { get; set; } = new UserRepository(context,userLogger);
    public IVariantRepository VariantRepository { get; set; } = new VariantRepository(context,variantLogger);

    public IOrderProductVariant OrderProductVariantRepository { get; set; } =
        new OrderProductVariantRepository(context);

    public IAnalyseRepository AnalyseRepository { get; set; } = new AnalyseRepository(context);
    public ICurrencyRepository CurrencyRepository { get; set; } = new CurrencyRepository(context,currencyLogger);
    public IPaymentTypeRepository PaymentTypeRepository { get; set; } = new PaymentTypeRepository(context,paymentTypeLogger);

    public IUserRefreshTokenRepository UserRefreshTokenRepository { get; set; } =
        new UserRefreshTokenRepository(context,userRefreshmentLogger);

    public void Dispose()
    {
        context.Dispose();
    }


    public async Task<int> SaveChanges()
    {
        try
        {
            return await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "operation error from execute that to database");
            return 0;
        }
    }
}