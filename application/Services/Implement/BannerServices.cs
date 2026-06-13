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
        logger.LogInformation("Start calling create banner function");
        var user = await unitOfWork.UserRepository
            .GetUser(userId);

        var validation = user.IsValidateFunc(false, true);

        if (validation is not null)
        {
            logger.LogInformation("validation error at  create banner function by userId {userId}", userId);

            return new ObjectResult(validation.Item1) { StatusCode = validation.Item2 };
        }


        var storeBannerCount = await unitOfWork.BannerRepository
            .GetBannerCount(user?.Store?.Id ?? ClsUtil.GenerateGuid());

        if (storeBannerCount >= 20 && user?.IsUser == true)
        {
            logger.LogError(
                "error at create banner function from {userId} and his store already had active {bannerNumber}", userId,
                storeBannerCount);
            return new ObjectResult("store can only have 20") { StatusCode = 404 };
        }


        var image = await fileServices.SaveFile(
            bannerDto.Image,
            EnImageType.Banner);

        if (image is null)
        {
            logger.LogError("error at create banner function from saved image to local and getting the url of it");

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
            logger.LogError("could not  create banner function  error from saved ef function");

            fileServices.DeleteFile(image);

            return new ObjectResult("error while adding new banner") { StatusCode = 500 };
        }

        await hubContext.Clients.All.SendAsync("createdBanner", result);


        await cache.RemoveByTagAsync(MemoryCacheKeys.BannersKey);

        var bannerToDto = banner.ToDto(config["url_file"] ?? "");


        logger.LogInformation("end calling create banner function");


        return new ObjectResult(bannerToDto) { StatusCode = 201 };
    }

    public async Task<IActionResult> DeleteBanner(Guid id, Guid userId)
    {
        logger.LogInformation("start calling delete banner function");

        var user = await unitOfWork.UserRepository
            .GetUser(userId);

        var validation = user.IsValidateFunc(false, true);
        if (validation is not null)
        {
            return new ObjectResult(validation.Item1) { StatusCode = validation.Item2 };
        }


        var banner = await unitOfWork.BannerRepository
            .GetBanner(id);


        if (banner is null)
        {
            logger.LogError("bannerId {bannerId} is not exists from delete banner function", id);

            return new ObjectResult("banner  not found") { StatusCode = 404 };
        }

        if (banner.StoreId != user!.Store!.Id)
        {
            logger.LogError("some one try to delete banner not belong to him with {userId} from delete banner function",
                user.Id);

            return new ObjectResult("only store owner can delete banner") { StatusCode = 403 };
        }

        unitOfWork.BannerRepository.Delete(id);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("could not delete banner from ef at from delete banner function");

            return new ObjectResult("error while deleting banner") { StatusCode = 500 };
        }

        fileServices.DeleteFile(banner.Image);

        await hubContext.Clients.All.SendAsync("deletedBanner", id);

        await cache.RemoveByTagAsync(MemoryCacheKeys.BannersKey);

        logger.LogInformation("end calling delete banner function");

        return new ObjectResult(null) { StatusCode = 204 };
    }

    public async Task<IActionResult> GetBannersAll(
        Guid adminId,
        int pageNumber,
        int pageSize)
    {
        logger.LogInformation("start calling get banners by adminId function");

        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validation = user.IsValidateFunc();

        if (validation is not null)
        {
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

        logger.LogInformation("end calling get banners by adminId function");

        return new ObjectResult(banners) { StatusCode = 200 };
    }

    public async Task<IActionResult> GetBanners(
        Guid userId,
        int pageNumber,
        int pageSize
    )
    {
        logger.LogInformation("start calling get banners by userId function");

        var store = await unitOfWork.StoreRepository.GetStoreByUserId(userId);

        if (store is null)
        {
            logger.LogError("not store found for {userId} banners by userId function", userId);

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


        logger.LogInformation("end calling get banners by userId function");

        return new ObjectResult(banners) { StatusCode = 200 };
    }

    public async Task<IActionResult> GetBanners(
        int randomLenght
    )
    {
        logger.LogInformation("start calling get banners random function");

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

        logger.LogInformation("end calling get banners random function");

        return new ObjectResult(banners) { StatusCode = 200 };
    }
}