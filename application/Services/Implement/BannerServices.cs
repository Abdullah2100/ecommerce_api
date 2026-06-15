using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.shared.mapper;
using api.shared.signalr;
using api.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Hybrid;

namespace api.application.Services.Implement;

public class BannerServices(
    IConfiguration config,
    IHubContext<BannerHub> hubContext,
    IUnitOfWork unitOfWork,
    IFileServices fileServices,
    HybridCache cache,
    ILogger<BannerServices> logger)
    : IBannerServices
{
    public async Task<IActionResult> CreateBanner(
        Guid userId,
        CreateBannerDto bannerDto
    )
    {
        logger.LogInformation("Start calling create banner");
        var user = await unitOfWork.UserRepository
            .GetUser(userId);

        var validation = user.IsValidateFunc(false, true);

        if (validation is not null)
        {
            logger.LogInformation("user not valid {userId} validationError {message}", userId, validation.Item2);

            return new ObjectResult(validation.Item1) { StatusCode = validation.Item2 };
        }


        var storeBannerCount = await unitOfWork.BannerRepository
            .GetBannerCount(user?.Store?.Id ?? ClsUtil.GenerateGuid());

        if (storeBannerCount >= 20 && user?.IsUser == true)
        {
            logger.LogWarning("user {userId} hit the limit of 20 active banner for store   {storeId}", userId,
                user.Store?.Id);
            return new ObjectResult("store can only have 20 active banners") { StatusCode = 404 };
        }


        var image = await fileServices.SaveFile(
            bannerDto.Image,
            EnImageType.Banner);

        if (image is null)
        {
            logger.LogError("error from saving image to local");

            return new ObjectResult("error while saving banner  image") { StatusCode = 500 };
        }

        var banner = new Banner
        {
            Id = ClsUtil.GenerateGuid(),
            EndAt = bannerDto.EndAt,
            CreatedAt = DateTime.Today,
            Image = image,
            StoreId = user!.Store!.Id,
        };

        unitOfWork.BannerRepository.Add(banner);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("coud not create banner {bannerId} ", banner.Id);

            fileServices.DeleteFile(image);

            return new ObjectResult("error while adding new banner") { StatusCode = 500 };
        }
        
        logger.LogInformation("create new  banner. bannerId :{bannerId} userId:{userId}", banner.Id, userId);

        await hubContext.Clients.All.SendAsync("createdBanner", result);


        await cache.RemoveByTagAsync(MemoryCacheKeys.BannersKey);

        var bannerToDto = banner.ToDto(config["url_file"] ?? "");


        logger.LogInformation("end calling create banner");


        return new ObjectResult(bannerToDto) { StatusCode = 201 };
    }

    public async Task<IActionResult> DeleteBanner(Guid id, Guid userId)
    {
        logger.LogInformation("start calling delete banner");

        var user = await unitOfWork.UserRepository
            .GetUser(userId);

        var validation = user.IsValidateFunc(false, true);
        if (validation is not null)
        {
            logger.LogInformation("user not valid {userId} validationError {message}", userId, validation.Item2);

            return new ObjectResult(validation.Item1) { StatusCode = validation.Item2 };
        }


        var banner = await unitOfWork.BannerRepository
            .GetBanner(id);


        if (banner is null)
        {
            logger.LogError("not exist banner {bannerId}", id);

            return new ObjectResult("banner  not found") { StatusCode = 404 };
        }

        if (banner.StoreId != user!.Store!.Id)
        {
            logger.LogError("user can't delete banner not belong to him {userStoreId} {bannerStoreId}",
                user.Store.Id, banner.StoreId);

            return new ObjectResult("only store owner can delete banner") { StatusCode = 403 };
        }

        unitOfWork.BannerRepository.Delete(id);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("could not delete banner {bannerId}", banner);

            return new ObjectResult("error while deleting banner") { StatusCode = 500 };
        }

        fileServices.DeleteFile(banner.Image);

        await hubContext.Clients.All.SendAsync("deletedBanner", id);

        await cache.RemoveByTagAsync(MemoryCacheKeys.BannersKey);

        logger.LogInformation("end calling delete banner");

        return new ObjectResult(null) { StatusCode = 204 };
    }

    public async Task<IActionResult> GetBannersAll(
        Guid adminId,
        int pageNumber,
        int pageSize)
    {
        logger.LogInformation("start calling get banners by adminId");

        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validation = user.IsValidateFunc();

        if (validation is not null)
        {
            logger.LogInformation("user not valid {userId} validationError {message}", adminId, validation.Item2);

            return new ObjectResult(validation.Item1) { StatusCode = validation.Item2 };
        }

        var banners = await cache.GetOrCreateAsync(MemoryCacheKeys.BannersKey + "/admin" + adminId + '/' + pageNumber,
            async ct =>
            {
                var banners = (await unitOfWork.BannerRepository
                        .GetBanners(pageNumber, pageSize)
                    )
                    .Select(ba => ba.ToDto(config["url_file"] ?? ""))
                    .ToList();
                return banners;
            },
            tags: [MemoryCacheKeys.BannersKey]);

        logger.LogInformation("end calling get banners by adminId");

        return new ObjectResult(banners) { StatusCode = 200 };
    }

    public async Task<IActionResult> GetBanners(
        Guid userId,
        int pageNumber,
        int pageSize
    )
    {
        logger.LogInformation("start calling get banners by userId");

        var store = await unitOfWork.StoreRepository.GetStoreByUserId(userId);

        if (store is null)
        {
            logger.LogError("user not having store");

            return new ObjectResult("store  not found") { StatusCode = 404 };
        }

        var banners = await cache.GetOrCreateAsync(MemoryCacheKeys.BannersKey + "/" + userId + '/' + pageNumber,
            async ct =>
            {
                var banners = (await unitOfWork.BannerRepository
                        .GetBanners(userId, pageNumber, pageSize))
                    .Select(ba => ba.ToDto(config["url_file"] ?? ""))
                    .ToList();
                return banners;
            },
            tags: [MemoryCacheKeys.BannersKey]);


        logger.LogInformation("end calling get banners by userId");

        return new ObjectResult(banners) { StatusCode = 200 };
    }

    public async Task<IActionResult> GetBanners(
        int randomLenght
    )
    {
        logger.LogInformation("start calling get banners random");

        var banners = await cache.GetOrCreateAsync(MemoryCacheKeys.BannersKey + "/" + randomLenght,
            async ct =>
            {
                var banners = (await unitOfWork.BannerRepository
                        .GetBanners(randomLenght))
                    .Select(ba => ba.ToDto(config["url_file"] ?? ""))
                    .ToList();
                return banners;
            },
            tags: [MemoryCacheKeys.BannersKey]);

        logger.LogInformation("end calling get banners random");

        return new ObjectResult(banners) { StatusCode = 200 };
    }
}