using api.application;
using api.application.Interface;
using api.application.Services;
using api.application.Services.Implement;
using api.application.Services.Interface;
using api.application.UnitOfWork;
using api.Infrastructure;
using api.Settings;
using Microsoft.Extensions.Options;

namespace api;

public  static class   DependencyInjection
{

    extension(IServiceCollection services)
    {
        public IServiceCollection AddServices()
        {
            services.AddTransient<IUserServices, UserService>();
            services.AddTransient<IStoreServices, StoreServices>();
            services.AddTransient<ICategoryServices, CategoryServices>();
            services.AddTransient<ISubCategoryServices, SubCategoryServices>();
            services.AddTransient<IVariantServices, VariantServices>();
            services.AddTransient<IBannerServices, BannerServices>();
            services.AddTransient<IGeneralSettingServices, GeneralSettingServices>();
            services.AddTransient<IDeliveryServices, DeliveryServices>();
            services.AddTransient<IProductServices, ProductServices>();
            services.AddTransient<IOrderServices, OrderServices>();
            services.AddTransient<IOrderItemServices, OrderItemServices>();
            services.AddTransient<IRefreshTokenServices, RefreshTokenServices>();
            services.AddTransient<IAnalyseServices, AnalyseServices>();
            services.AddTransient<ICurrencyServices, CurrencyServices>();
            services.AddTransient<IPaymentTypeServices, PaymentTypeServices>();
            services.AddTransient<IFileServices, FileServices>();
            
            services.AddKeyedScoped<IMessageService, EmailServices>(EnMessageService.Email);
            services.AddKeyedScoped<IMessageService, NotificationServices>(EnMessageService.Notification);

            services.AddScoped<IAuthenticationService, AuthenticationServices>();
            
            services.AddTransient<IPaymentServices, StripPaymentServices>();


            return  services;
        }

        public IServiceCollection AddUnitOfWork()
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            return services;
        }
        
        public IServiceCollection AddSetting()
        {
            services.AddScoped<SmtpSetting>(sp=>sp.GetRequiredService<IOptions<SmtpSetting>>().Value);
            services.AddScoped<StripeSetting>(sp=>sp.GetRequiredService<IOptions<StripeSetting>>().Value);
            services.AddScoped<CredentialSetting>(sp=>sp.GetRequiredService<IOptions<CredentialSetting>>().Value);
            return services;
        }
        
        
    }
}