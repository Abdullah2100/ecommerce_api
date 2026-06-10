using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using api.application;
using api.application.Services.Implement;
using api.application.Services.Interface;
using api.application.UnitOfWork;
using api.Filter;
using api.Infrastructure;
using api.OpenApi.Transformers;
using api.Settings;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace api;

public static class DependencyInjection
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


            return services;
        }

        public IServiceCollection AddUnitOfWork()
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public IServiceCollection AddSetting()
        {
            services.AddScoped<SmtpSetting>(sp => sp.GetRequiredService<IOptions<SmtpSetting>>().Value);
            services.AddScoped<StripeSetting>(sp => sp.GetRequiredService<IOptions<StripeSetting>>().Value);
            services.AddScoped<CredentialSetting>(sp => sp.GetRequiredService<IOptions<CredentialSetting>>().Value);
            return services;
        }

        public IServiceCollection AddApiDocumentation()
        {
            var versions = new List<string>() { "v1", "v2" };
            foreach (var version in versions)
            {
                services.AddOpenApi(version, option =>
                {
                    option.AddDocumentTransformer<VersionInfoTransformer>();
                    option.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
                    option.AddOperationTransformer<BearerSecuritySchemeTransformer>();
                });
            }

            return services;
        }


        public IServiceCollection AddJwtAuthentication(IConfiguration config)
        {
            var credential = config
                .GetSection(CredentialSetting.Name)
                .Get<CredentialSetting>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(credential?.key ?? "")
                        ),
                        ValidIssuer = credential?.Issuer ?? "",
                        ValidAudience = credential?.Audience ?? ""
                    };
                });
            return services;
        }

        public IServiceCollection AddDbConnection(IConfiguration config)
        {
            var connectionUrl = config["ConnectionStrings:connection_url"];

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionUrl));
            return services;
        }

        public IServiceCollection AddCors(string corsName)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(corsName, policy =>
                {
                    policy
                        .WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
            return services;
        }

        public IServiceCollection AddController()
        {
            services.AddControllers(option =>
                option.Filters.Add(new CustomResultFilter())
            ).AddJsonOptions(option=>
            {
                option.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
                option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            return services;
        }

        public IServiceCollection AddSignalRService()
        {
            services.AddSignalR(option =>
                option.EnableDetailedErrors = true
            );
            return services;
        }

        public IServiceCollection AddVersions()
        {
            services.AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1);
                    options.ReportApiVersions = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ApiVersionReader = ApiVersionReader.Combine(
                        new UrlSegmentApiVersionReader(),
                        new HeaderApiVersionReader("X-Api-Version"));
                }).AddMvc()
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });
            return services;
        }

        public IServiceCollection AddCaching(IConfiguration config)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.GetConnectionString("redis");
                options.InstanceName = "s1";
            });
            services.AddHybridCache(options =>
            {
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(10),
                    LocalCacheExpiration = TimeSpan.FromMinutes(30),
                };
            });
            return services;
        }
        public IServiceCollection AddRateLimit()
        {
            services.AddRateLimiter(option =>
            {
            
                option.AddPolicy("userAccessLimit", httpContext =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey:httpContext.User.Identity?.Name??"anonymous",
                        factory:_=> new SlidingWindowRateLimiterOptions()
                        {
                            Window = TimeSpan.FromSeconds(5),
                            PermitLimit = 20,
                            AutoReplenishment = true,
                        }
                        )
                    );
                option.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 50,
                            Window = TimeSpan.FromMinutes(1)
                        }));
                
            });
            return services;
        } 
    }
}