using System.Text;
using api;
using api.application;
using api.application.Interface;
using api.application.Services;
using api.application.Services.Implement;
using api.application.Services.Interface;
using api.application.UnitOfWork;
using api.domain.Interface;
using api.Exceptions;
using api.Filter;
using api.Infrastructure;
using api.Infrastructure.Repositories;
using api.Settings;
using api.shared.midleware;
using api.shared.signalr;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


builder.Services.AddOptions();

//setting
builder.Services.AddSetting();


//unit of Work
builder.Services.AddUnitOfWork();

//services
builder.Services.AddServices();

//payment 
/*

var fireBaseConfig = Path.Combine(
    Directory.GetCurrentDirectory(),
    "librarynotification-notification.json"
);

var firebaseCredential = GoogleCredential.FromFile(fireBaseConfig);
FirebaseApp.Create(new AppOptions()
{
    Credential = firebaseCredential
});
*/


const string corsName = "AllowAllOrigins";
builder.Services.AddCors(options =>
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

builder.Services.AddControllers(option =>
    option.Filters.Add(new CustomResultFilter())
);
builder.Services.AddSignalR(option =>
    option.EnableDetailedErrors = true
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var credential = builder
    .Configuration
    .GetSection(CredentialSetting.Name)
    .Get<CredentialSetting>();

var stripeKey = builder
    .Configuration
    .GetSection(StripeSetting.Name)
    .Get<StripeSetting>();

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

// Database
var connectionUrl = configuration["ConnectionStrings:connection_url"];
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionUrl));


//stripe services
builder.Services.AddSingleton(new StripeClient(stripeKey?.SecretKey));

//exception service
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();
StripeConfiguration.ApiKey = stripeKey?.SecretKey;


app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}
//           AllowAllOrigins


app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "images")),
    RequestPath = "/StaticFiles"
});
app.UseRouting();
app.UseCors(corsName);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<BannerHub>("/bannerHub");
app.MapHub<OrderHub>("/orderHub");
app.MapHub<OrderItemHub>("/orderItemHub");
app.MapHub<StoreHub>("/storeHub");
app.ConfigureExceptionHandler();
app.Run();