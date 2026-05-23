using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.shared.mapper;
using api.shared.signalr;
using api.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace api.application.Services.Implement;

public class BannerServices(
    IConfiguration config,
    IHubContext<BannerHub> hubContext,
    IUnitOfWork unitOfWork,
    IFileServices fileServices)
    : IBannerServices
{
    public async Task<IActionResult> CreateBanner(
        Guid userId,
        CreateBannerDto bannerDto
    )
    {
        var user = await unitOfWork.UserRepository
            .GetUser(userId);

        var validation = user.IsValidateFunc(false, true);

        if (validation is not null)
        {
            return new ObjectResult(validation.Item1) { StatusCode = validation.Item2 };
        }


        var storeBannerCount = await unitOfWork.BannerRepository
            .GetBannerCount(user?.Store?.Id ?? ClsUtil.GenerateGuid());

        if (storeBannerCount >= 20 && user?.IsUser == true)
        {
            return new ObjectResult("store can only have 20") { StatusCode = 404 };
        }


        var image = await fileServices.SaveFile(
            bannerDto.Image,
            EnImageType.Banner);

        if (image is null)
        {
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
            fileServices.DeleteFile(image);

            return new ObjectResult("error while adding new banner") { StatusCode = 500 };
        }

        await hubContext.Clients.All.SendAsync("createdBanner", result);

        var bannerToDto = banner.ToDto(config["url_file"]??"");
        return new ObjectResult(bannerToDto) { StatusCode = 201 };
    }

    public async Task<IActionResult> DeleteBanner(Guid id, Guid userId)
    {
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
            return new ObjectResult("banner  not found") { StatusCode = 404 };
        }

        if (banner.StoreId != user!.Store!.Id)
        {
            return new ObjectResult("only store owner can delete banner") { StatusCode = 403 };
        }

        unitOfWork.BannerRepository.Delete(id);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while deleting banner") { StatusCode = 500 };
        }

        fileServices.DeleteFile(banner.Image);

        await hubContext.Clients.All.SendAsync("deletedBanner", id);


        return new ObjectResult(null) { StatusCode = 204 };
    }

    public async Task<IActionResult> GetBannersAll(
        Guid adminId,
        int pageNumber,
        int pageSize)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(adminId);
        var validation = user.IsValidateFunc();
        if (validation is not null)
        {
            return new ObjectResult(validation.Item1) { StatusCode = validation.Item2 };
        }


        var banners = (await unitOfWork.BannerRepository
                .GetBanners(pageNumber, pageSize)
            )
            .Select(ba => ba.ToDto(config["url_file"]??""))
            .ToList();

        return new ObjectResult(banners) { StatusCode = 200 };
    }

    public async Task<IActionResult> GetBanners(
        Guid userId,
        int pageNumber,
        int pageSize
    )
    {
        var store = await unitOfWork.StoreRepository.GetStoreByUserId(userId);

        if (store is null)
        {
            return new ObjectResult("store  not found") { StatusCode = 404 };
        }
        
        var banners = (await unitOfWork.BannerRepository
                .GetBanners(userId, pageNumber, pageSize))
            .Select(ba => ba.ToDto(config["url_file"]??""))
            .ToList();

        return new ObjectResult(banners) { StatusCode = 200 };
    }

    public async Task<IActionResult> GetBanners(
        int randomLenght
    )
    {
        var banners = (await unitOfWork.BannerRepository
                .GetBanners(randomLenght))
            .Select(ba => ba.ToDto(config["url_file"]??""))
            .ToList();

        return new ObjectResult(banners) { StatusCode = 200 };
    }
}