using api;
using api.Exceptions;
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
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
        options.RoutePrefix = string.Empty;
    });
}



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