using api.domain.Interface;
using api.Infrastructure;
using api.Infrastructure.Repositories;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace api.application.UnitOfWork;

public class UnitOfWork(ILogger<UnitOfWork> logger, AppDbContext context) : IUnitOfWork
{
    public IAddressRepository AddressRepository { get; set; } = new AddressRepository(context);
    public IBannerRepository BannerRepository { get; set; } = new BannerRepository(context);
    public ICategoryRepository CategoryRepository  { get; set; }= new CategoryRepository(context);
    public IDeliveryRepository DeliveryRepository { get; set; } = new DeliveryRepository(context);
    public IGeneralSettingRepository GeneralSettingRepository { get; set; } = new GeneralSettingRepository(context);
    public IOrderItemRepository OrderItemRepository { get; set; } = new OrderItemRepository(context);
    public IProductVariantRepository ProductVariantRepository { get; set; } = new ProductVariantRepository(context);
    public IProductImageRepository ProductImageRepository  { get; set; }= new ProductImageRepository(context);
    public IProductRepository ProductRepository { get; set; } = new ProductRepository(context);
    public IOrderRepository OrderRepository { get; set; } = new OrderRepository(context);
    public IReseatePasswordRepository PasswordRepository { get; set; } = new ReseatPasswordRepository(context);
    public IStoreRepository StoreRepository { get; set; } = new StoreRepository(context);
    public ISubCategoryRepository SubCategoryRepository  { get; set; }= new SubCategoryRepository(context);
    public IUserRepository UserRepository { get; set; } = new UserRepository(context);
    public IVarientRepository VariantRepository { get; set; } = new VariantRepository(context);
    public IOrderProductVariant OrderProductVariantRepository { get; set; } = new OrderProductVariantRepository(context);
    public IAnalyseRepository AnalyseRepository { get; set; } = new AnalyseRepository(context);
    public ICurrencyRepository CurrencyRepository  { get; set; }= new CurrencyRepository(context);
    public IPaymentTypeRepository PaymentTypeRepository { get; set; } = new PaymentTypeRepository(context);
    public IUserRefreshTokenRepository UserRefreshTokenRepository { get; set; } = new UserRefreshTokenRepository(context);

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