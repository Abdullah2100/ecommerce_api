using api.Infrastructure;
using business.Services.Interface;

namespace api.backgroundServices
{
    public class DeleteBannerImageBackgroundService(
        IUnitOfWork unitOfWork,
        IFileServices services,
        IWebHostEnvironment environment,
        ILogger<DeleteBannerImageBackgroundService> logger) : BackgroundService
    {
        private readonly TimeSpan _itrivel = TimeSpan.FromHours(12);
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("start  delete images background services");

            var preidctTimer = new PeriodicTimer(_itrivel);
            while (await preidctTimer.WaitForNextTickAsync())
            {
                try
                {
                    var listBanner = await unitOfWork.BannerRepository.GetNotActiveBanners(20);
                    if (listBanner.Count > 0)
                    {
                        var rootPath = environment.WebRootPath;
                        foreach (var banner in listBanner)
                        {
                            services.DeleteFile(banner.Image,rootPath);
                            unitOfWork.BannerRepository.Delete(banner.Id);

                        }

                        await unitOfWork.SaveChanges();
                    }

                }
                catch (Exception ex)
                {

                    logger.LogError(ex, "error from deleting image background services is {errorMessage}", ex.Message);
                }
            }

            logger.LogInformation("End delete image background services");
        }
    }
}