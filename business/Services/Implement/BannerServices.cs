using api.application;
using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using business.mapper;
using api.util;
using business.Services.Interface;
using data.dto.Request;
using data.util;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace business.Services.Implement;

public class BannerServices(
    IConfiguration config,
    IUnitOfWork unitOfWork,
    IFileServices fileServices,
    HybridCache cache,
    ILogger<BannerServices> logger)
    : IBannerServices
{
    public async Task<Result> CreateBanner(Guid userId, CreateBannerDto bannerDto,string rootPath,Action<int> sendSocketMessage)
    {
        logger.LogInformation("Start calling create banner");
        var user = await unitOfWork.UserRepository.GetUser(userId);

        var validation = user.IsValidateFunc(false, true);

        if (validation is not null)
        {
            logger.LogInformation("user not valid {userId} validationError {message}", userId, validation.Item2);
            return new Result(false, validation.Item1, null, validation.Item2);
        }

        var storeBannerCount = await unitOfWork.BannerRepository.GetBannerCount(user?.Store?.Id ?? ClsUtil.GenerateGuid());

        if (storeBannerCount >= 20 && user?.IsUser == true)
        {
            logger.LogWarning("user {userId} hit the limit of 20 active banner for store   {storeId}", userId, user.Store?.Id);
            return new Result(false, "store can only have 20 active banners", null, 404);
        }

        var image = await fileServices.SaveFile(bannerDto.Image, EnImageType.Banner,rootPath);

        if (image is null)
        {
            logger.LogError("error from saving image to local");
            return new Result(false, "error while saving banner  image", null, 500);
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
            fileServices.DeleteFile(image,rootPath);
            return new Result(false, "error while adding new banner", null, 500);
        }

        logger.LogInformation("create new  banner. bannerId :{bannerId} userId:{userId}", banner.Id, userId);
        sendSocketMessage.Invoke(result);
        await cache.RemoveByTagAsync(MemoryCacheKeys.BannersKey);

        var bannerToDto = banner.ToDto(config["url_file"] ?? "");
        logger.LogInformation("end calling create banner");
        return new Result(true, null, bannerToDto, 201);
    }

    public async Task<Result> DeleteBanner(Guid id, Guid userId,string rootPath,Action<Guid>sendDeleteSocket)
    {
        logger.LogInformation("start calling delete banner");

        var user = await unitOfWork.UserRepository.GetUser(userId);
        var validation = user.IsValidateFunc(false, true);
        if (validation is not null)
        {
            logger.LogInformation("user not valid {userId} validationError {message}", userId, validation.Item2);
            return new Result(false, validation.Item1, null, validation.Item2);
        }

        var banner = await unitOfWork.BannerRepository.GetBanner(id);

        if (banner is null)
        {
            logger.LogError("not exist banner {bannerId}", id);
            return new Result(false, "banner  not found", null, 404);
        }

        if (banner.StoreId != user!.Store!.Id)
        {
            logger.LogError("user can't delete banner not belong to him {userStoreId} {bannerStoreId}", user.Store.Id, banner.StoreId);
            return new Result(false, "only store owner can delete banner", null, 403);
        }

        unitOfWork.BannerRepository.Delete(id);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("could not delete banner {bannerId}", banner);
            return new Result(false, "error while deleting banner", null, 500);
        }

        fileServices.DeleteFile(banner.Image,rootPath);
        sendDeleteSocket.Invoke(id);
        await cache.RemoveByTagAsync(MemoryCacheKeys.BannersKey);

        logger.LogInformation("end calling delete banner");
        return new Result(true, null, null, 204);
    }

    public async Task<Result> GetBannersAll(Guid adminId, int pageNumber, int pageSize)
    {
        logger.LogInformation("start calling get banners by adminId");

        var user = await unitOfWork.UserRepository.GetUser(adminId);
        var validation = user.IsValidateFunc();

        if (validation is not null)
        {
            logger.LogInformation("user not valid {userId} validationError {message}", adminId, validation.Item2);
            return new Result(false, validation.Item1, null, validation.Item2);
        }

        var banners = await cache.GetOrCreateAsync(MemoryCacheKeys.BannersKey + "/admin" + adminId + '/' + pageNumber,
            async ct =>
            {
                var banners = (await unitOfWork.BannerRepository.GetBanners(pageNumber, pageSize))
                    .Select(ba => ba.ToDto(config["url_file"] ?? ""))
                    .ToList();
                return banners;
            },
            tags: [MemoryCacheKeys.BannersKey]);

        logger.LogInformation("end calling get banners by adminId");
        return new Result(true, null, banners, 200);
    }

    public async Task<Result> GetBanners(Guid userId, int pageNumber, int pageSize)
    {
        logger.LogInformation("start calling get banners by userId");

        var store = await unitOfWork.StoreRepository.GetStoreByUserId(userId);

        if (store is null)
        {
            logger.LogError("user not having store");
            return new Result(false, "store  not found", null, 404);
        }

        var banners = await cache.GetOrCreateAsync(MemoryCacheKeys.BannersKey + "/" + userId + '/' + pageNumber,
            async ct =>
            {
                var banners = (await unitOfWork.BannerRepository.GetBanners(userId, pageNumber, pageSize))
                    .Select(ba => ba.ToDto(config["url_file"] ?? ""))
                    .ToList();
                return banners;
            },
            tags: [MemoryCacheKeys.BannersKey]);

        logger.LogInformation("end calling get banners by userId");
        return new Result(true, null, banners, 200);
    }

    public async Task<Result> GetBanners(int randomLenght)
    {
        logger.LogInformation("start calling get banners random");

        var banners = await cache.GetOrCreateAsync(MemoryCacheKeys.BannersKey + "/" + randomLenght,
            async ct =>
            {
                var banners = (await unitOfWork.BannerRepository.GetBanners(randomLenght))
                    .Select(ba => ba.ToDto(config["url_file"] ?? ""))
                    .ToList();
                return banners;
            },
            tags: [MemoryCacheKeys.BannersKey]);

        logger.LogInformation("end calling get banners random");
        return new Result(true, null, banners, 200);
    }
}