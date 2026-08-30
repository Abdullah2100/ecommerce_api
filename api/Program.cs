using api;
using api.Exceptions;
using api.Filter;
using api.Settings;
using api.shared.midleware;
using api.shared.signalr;
using Microsoft.Extensions.FileProviders;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

//openApi
builder.Services.AddApiDocumentation();


//option pattern
builder.Services.AddOptions();

//setting
builder.Services.AddSetting();


//unit of Work
builder.Services.AddUnitOfWork();

//services
builder.Services.AddServices();


//rate limit 
builder.Services.AddRateLimit();

//firebase 
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
builder.Services.AddCors(corsName);

builder.Services.AddController();

builder.Services.AddSignalRService();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddVersions();

builder.Services.AddCaching(configuration);

builder.Services.AddJwtAuthentication(configuration);


var stripeKey = builder
    .Configuration
    .GetSection(StripeSetting.Name)
    .Get<StripeSetting>();


// Database
builder.Services.AddDbConnection(configuration);

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
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "ِApi V1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Api V2");
        options.RoutePrefix = string.Empty;
    });
}


app.UseHttpsRedirection();
var imagePath = Path.Combine(builder.Environment.ContentRootPath,"images");
Directory.CreateDirectory(imagePath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(imagePath),
    RequestPath = "/StaticFiles"
});
app.UseRouting();
app.UseCors(corsName);
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers()
.RequireRateLimiting("userAccessLimit");
app.MapHub<BannerHub>("/bannerHub");
app.MapHub<OrderHub>("/orderHub");
app.MapHub<OrderItemHub>("/orderItemHub");
app.MapHub<StoreHub>("/storeHub");
app.ConfigureExceptionHandler();
app.Run();