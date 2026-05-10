using api.application.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using api.shared.mapper;
using api.shared.signalr;
using api.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace api.application.Services;

public class BannerServices(
    IConfig config,
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
        User? user = await unitOfWork.UserRepository
            .GetUser(userId);

        var validation = user.IsValidateFunc(false, true);

        if (validation is not null)
        {
            return new ObjectResult(validation.Item1) { StatusCode = validation.Item2 };
        }


        /*this to remove some banner to be away from overload of banner to keep vps fit to size
        int bannersCount = await unitOfWork.BannerRepository.GetBannerCount();
        if (bannersCount > 20)
        {
            var bannersRandom = await unitOfWork.BannerRepository.GetBanners(20);
            var imagesList = bannersRandom.Select(b => b.Image).ToList();
            fileServices.DeleteFile(imagesList);
            unitOfWork.BannerRepository.Delete(bannersRandom);
        }*/
        //end 


        //this for api  to prevent user have more that 20 banners
        var storeBannerCount = await unitOfWork.BannerRepository
            .GetBannerCount(user?.Store?.Id ?? ClsUtil.GenerateGuid());

        if (storeBannerCount >= 20 && user?.IsUser == true)
        {
            return new ObjectResult("store can only have 20") { StatusCode = 404 };
        }


        string? image = await fileServices.SaveFile(
            bannerDto.Image,
            EnImageType.Banner);

        if (image is null)
        {
            return new ObjectResult("error while saving banner  image") { StatusCode = 500 };
        }

        Banner banner = new Banner
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

        var bannerToDto = banner.ToDto(config.GetKey("url_file"));
        return new ObjectResult(bannerToDto) { StatusCode = 201 };
    }

    public async Task<IActionResult> DeleteBanner(Guid id, Guid userId)
    {
        User? user = await unitOfWork.UserRepository
            .GetUser(userId);

        var validation = user.IsValidateFunc(false, true);
        if (validation is not null)
        {
            return new ObjectResult(validation.Item1) { StatusCode = validation.Item2 };
        }


        Banner? banner = await unitOfWork.BannerRepository
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
        int result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while deleting banner") { StatusCode = 500 };
        }

        fileServices.DeleteFile(banner.Image);

        await hubContext.Clients.All.SendAsync("deletedOrder", id);


        return new ObjectResult(null) { StatusCode = 204 };
    }

    public async Task<IActionResult> GetBannersAll(
        Guid adminId,
        int pageNumber,
        int pageSize)
    {
        User? user = await unitOfWork.UserRepository
            .GetUser(adminId);
        var validation = user.IsValidateFunc();
        if (validation is not null)
        {
            return new ObjectResult(validation.Item1) { StatusCode = validation.Item2 };
        }


        List<BannerDto> banners = (await unitOfWork.BannerRepository
                .GetBanners(pageNumber, pageSize)
            )
            .Select(ba => ba.ToDto(config.GetKey("url_file")))
            .ToList();

        return new ObjectResult(banners) { StatusCode = 200 };
    }

    public async Task<IActionResult> GetBanners(
        Guid storeId,
        int pageNumber,
        int pageSize
    )
    {
        List<BannerDto> banners = (await unitOfWork.BannerRepository
                .GetBanners(storeId, pageNumber, pageSize))
            .Select(ba => ba.ToDto(config.GetKey("url_file")))
            .ToList();

        return new ObjectResult(banners) { StatusCode = 200 };
    }

    public async Task<IActionResult> GetBanners(
        int randomLenght
    )
    {
        List<BannerDto> banners = (await unitOfWork.BannerRepository
                .GetBanners(randomLenght))
            .Select(ba => ba.ToDto(config.GetKey("url_file")))
            .ToList();

        return new ObjectResult(banners) { StatusCode = 200 };
    }
}