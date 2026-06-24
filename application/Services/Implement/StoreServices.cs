using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using api.shared.mapper;
using api.shared.signalr;
using api.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Hybrid;

namespace api.application.Services.Implement;

public class StoreServices(
    IWebHostEnvironment host,
    IConfiguration config,
    IFileServices fileServices,
    IUnitOfWork unitOfWork,
    IHubContext<StoreHub> hubContext,
    HybridCache cache 
)
    : IStoreServices
{
    public async Task<IActionResult> GetStores(Guid adminId, string prefix, int pageSize)
    {
        var stores = await cache.GetOrCreateAsync(
            MemoryCacheKeys.StoresKey + "/" + adminId + "/" + prefix + '/' + pageSize,
            async ct =>
            {
                var stores = (await unitOfWork.StoreRepository
                        .GetStores(prefix, pageSize))
                    .Select(st => st.ToDto(config["url_file"] ?? ""))
                    .ToList();

                return stores;
            },
            tags: [MemoryCacheKeys.StoresKey]);


        return new ObjectResult(stores)
            { StatusCode = StatusCodes.Status200OK };
    }


    private void DeleteStoreImage(string? wallpaper, string? smallImage)
    {
        if (wallpaper is not null)
            fileServices.DeleteFile(wallpaper);
        if (smallImage is not null)
            fileServices.DeleteFile(smallImage);
    }

    public async Task<IActionResult> CreateStore(
        CreateStoreDto store,
        Guid userId)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(userId);

        var validationResult = user.IsValidateFunc();

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }


        if (await unitOfWork.StoreRepository.IsExist(store.Name))
        {
            return new ObjectResult("store name have been already exist")
                { StatusCode = StatusCodes.Status409Conflict };
        }

        string? wallpaper = null, smallImage = null;

        smallImage = await fileServices.SaveFile(
            store.SmallImage,
            EnImageType.Store);

        wallpaper = await fileServices.SaveFile(
            store.WallpaperImage,
            EnImageType.Store);


        if (smallImage is null || wallpaper is null)
        {
            DeleteStoreImage(wallpaper, smallImage);

            return new ObjectResult("error while saving store images")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var id = ClsUtil.GenerateGuid();

        var storeData = new Store
        {
            Id = id,
            Name = store.Name,
            WallpaperImage = wallpaper,
            SmallImage = smallImage,
            IsBlock = user?.IsUser != false,
            UserId = userId,
            CreatedAt = DateTime.Now,
            UpdatedAt = null,
        };

        var address = new Address
        {
            Id = ClsUtil.GenerateGuid(),
            IsCurrent = true,
            Latitude = store.Latitude,
            Longitude = store.Longitude,
            Title = store.Name,
            CreatedAt = DateTime.Now,
            UpdatedAt = null,
            OwnerId = id
        };

        unitOfWork.StoreRepository.Add(storeData);


        unitOfWork.AddressRepository.Add(address);


        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            DeleteStoreImage(wallpaper, smallImage);
            fileServices.DeleteFile([wallpaper, smallImage]);

            return new ObjectResult("error while adding store")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        storeData = await unitOfWork.StoreRepository.GetStore(id)!;
        storeData!.Addresses = new List<Address> { address };


        var storeToDto = storeData?.ToDto(config["url_file"] ?? "");

        await cache.RemoveByTagAsync(MemoryCacheKeys.StoresKey);

        return new ObjectResult(storeToDto)
            { StatusCode = StatusCodes.Status201Created };
    }

    public async Task<IActionResult> UpdateStore(
        UpdateStoreDto storeDto,
        Guid userId
    )
    {
        if (storeDto.IsEmpty())
        {
            return new ObjectResult("no found update at data request")
                { StatusCode = StatusCodes.Status400BadRequest };
        }

        var user = await unitOfWork.UserRepository
            .GetUser(userId);

        var validationResult = user.IsValidateFunc(isStore: true);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        if (storeDto.Name is not null)
        {
            var isExist = await unitOfWork.StoreRepository.IsExist(storeDto.Name, user!.Store!.Id);

            if (isExist)
            {
                return new ObjectResult("store name have been already exist")
                    { StatusCode = StatusCodes.Status409Conflict };
            }
        }


        string? wallpaper = null, smallImage = null;

        if (storeDto.WallpaperImage is not null)
        {
            wallpaper = await fileServices.SaveFile(
                storeDto.WallpaperImage,
                EnImageType.Store);

            DeleteStoreImage(user!.Store!.WallpaperImage, null);
        }

        if (storeDto.SmallImage is not null)
        {
            smallImage = await fileServices.SaveFile(
                storeDto.SmallImage,
                EnImageType.Store);
            DeleteStoreImage(null, user!.Store?.SmallImage);
        }

        user!.Store!.SmallImage = smallImage ?? user!.Store!.SmallImage;
        user!.Store!.WallpaperImage = wallpaper ?? user!.Store!.WallpaperImage;
        user!.Store!.Name = storeDto.Name ?? user!.Store!.Name;
        user!.Store!.UpdatedAt = DateTime.Now;

        unitOfWork.StoreRepository.Update(user!.Store!);

        if (
            (storeDto.Longitude is null && storeDto.Latitude is not null) ||
            (storeDto.Longitude is not null && storeDto.Latitude is null)
        )
        {
            fileServices.DeleteFile([wallpaper ?? "", smallImage ?? ""]);


            return new ObjectResult(
                    "when update address you must change both longitude and latitude not one of them only")
                { StatusCode = StatusCodes.Status400BadRequest };
        }


        var result = await unitOfWork.SaveChanges();

        if (result < 1)
        {
            return new ObjectResult("could not update store") { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var store = await unitOfWork.StoreRepository.GetStore(user.Store.Id);
        store?.Addresses = await unitOfWork.AddressRepository.GetAllAddressByOwnerId(store!.Id);

        await cache.RemoveByTagAsync(MemoryCacheKeys.StoresKey);


        return new ObjectResult(null) { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> GetStorePage(Guid adminId, int storePerPage)

    {
        var store = await unitOfWork.UserRepository.GetUser(adminId);

        var validationResult = store.IsValidateFunc();

        if (validationResult is not null)
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };

        var count = await unitOfWork.StoreRepository.GetStoresCount(storePerPage);

        return new ObjectResult(count)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }


    public async Task<IActionResult> GetStoreByUserId(Guid userId)
    {
        var store = await unitOfWork.StoreRepository.GetStoreByUserId(userId);

        if (store is null)
            return new ObjectResult("store not found")
                { StatusCode = StatusCodes.Status404NotFound };


        var storeToDto = store.ToDto(config["url_file"] ?? "");
        return new ObjectResult(storeToDto)
            { StatusCode = StatusCodes.Status200OK };
    }


    public async Task<IActionResult> GetStoreByStoreId(Guid id)
    {
        var store = await unitOfWork.StoreRepository.GetStore(id);

        if (store is null)
            return new ObjectResult("store not found")
                { StatusCode = StatusCodes.Status404NotFound };


        var storeToDto = store.ToDto(config["url_file"] ?? "");
        return new ObjectResult(storeToDto)
            { StatusCode = StatusCodes.Status200OK };
    }


    public async Task<IActionResult> GetStores(Guid adminId, int pageNumber, int pageSize)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc(true);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var stores = await cache.GetOrCreateAsync(
            MemoryCacheKeys.StoresKey + "/" + adminId + '/' + pageNumber,
            async ct =>
            {
                var stores = (await unitOfWork.StoreRepository
                        .GetStores(pageNumber, pageSize)
                    ).Select(st => st.ToDto(config["url_file"] ?? ""))
                    .ToList();

                return stores;
            },
            tags: [MemoryCacheKeys.StoresKey]);


        return new ObjectResult(stores)
            { StatusCode = StatusCodes.Status200OK };
    }

    public async Task<IActionResult> UpdateStoreStatus(Guid adminId, Guid storeId)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc(false, isStore: true);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var store = await unitOfWork.StoreRepository.GetStore(storeId);


        validationResult = store!.user.IsValidateFunc(true);

        if (validationResult is null && store.UserId != user!.Id)
        {
            return new ObjectResult("only Admin can update his store Status")
                { StatusCode = StatusCodes.Status403Forbidden };
        }


        store.IsBlock = !store.IsBlock;

        if (store.IsBlock && user?.IsUser == false)
        {
            return new ObjectResult("this store is belong to admin you could not block it ")
                { StatusCode = StatusCodes.Status403Forbidden };
        }

        unitOfWork.StoreRepository.Update(store);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
            return new ObjectResult("error while update store status")
                { StatusCode = StatusCodes.Status500InternalServerError };

        await hubContext.Clients.All.SendAsync("storeStatus", new StoreStatusDto
        {
            StoreId = storeId,
            Status = true
        });
        await cache.RemoveByTagAsync(MemoryCacheKeys.StoresKey);

        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }
}