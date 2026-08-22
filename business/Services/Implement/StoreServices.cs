using api.application;
using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using business.mapper;
using api.util;
using business.Services.Interface;
using data.dto.Request;
using data.dto.Response;
using data.util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;

namespace business.Services.Implement;

public class StoreServices(
    IWebHostEnvironment host,
    IConfiguration config,
    IFileServices fileServices,
    IUnitOfWork unitOfWork,
    
    HybridCache cache 
)
    : IStoreServices
{
    public async Task<Result> GetStores(Guid adminId, string prefix, int pageSize)
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

        return new Result(true, null, stores, 200);
    }

    private void DeleteStoreImage(string? wallpaper, string? smallImage,string rootPath)
    {
        if (wallpaper is not null)
            fileServices.DeleteFile(wallpaper,rootPath);
        if (smallImage is not null)
            fileServices.DeleteFile(smallImage,rootPath);
    }

    public async Task<Result> CreateStore(CreateStoreDto store, Guid userId,string rootPath)
    {
        var user = await unitOfWork.UserRepository.GetUser(userId);

        var validationResult = user.IsValidateFunc();

        if (validationResult is not null)
        {
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        if (await unitOfWork.StoreRepository.IsExist(store.Name))
        {
            return new Result(false, "store name have been already exist", null, 409);
        }

        string? wallpaper = null, smallImage = null;

        smallImage = await fileServices.SaveFile(store.SmallImage, EnImageType.Store,rootPath);
        wallpaper = await fileServices.SaveFile(store.WallpaperImage, EnImageType.Store,rootPath);

        if (smallImage is null || wallpaper is null)
        {
            DeleteStoreImage(wallpaper, smallImage,rootPath);
            return new Result(false, "error while saving store images", null, 500);
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
            DeleteStoreImage(wallpaper, smallImage,rootPath);
            fileServices.DeleteFile([wallpaper, smallImage],rootPath);
            return new Result(false, "error while adding store", null, 500);
        }

        storeData = await unitOfWork.StoreRepository.GetStore(id)!;
        storeData!.Addresses = new List<Address> { address };

        var storeToDto = storeData?.ToDto(config["url_file"] ?? "");

        await cache.RemoveByTagAsync(MemoryCacheKeys.StoresKey);

        return new Result(true, null, storeToDto, 201);
    }

    public async Task<Result> UpdateStore(UpdateStoreDto storeDto, Guid userId,string rootPath)
    {
        if (storeDto.IsEmpty())
        {
            return new Result(false, "no found update at data request", null, 400);
        }

        var user = await unitOfWork.UserRepository.GetUser(userId);
        var validationResult = user.IsValidateFunc(isStore: true);

        if (validationResult is not null)
        {
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        if (storeDto.Name is not null)
        {
            var isExist = await unitOfWork.StoreRepository.IsExist(storeDto.Name, user!.Store!.Id);
            if (isExist)
            {
                return new Result(false, "store name have been already exist", null, 409);
            }
        }

        string? wallpaper = null, smallImage = null;

        if (storeDto.WallpaperImage is not null)
        {
            wallpaper = await fileServices.SaveFile(storeDto.WallpaperImage, EnImageType.Store,rootPath);
            DeleteStoreImage(user!.Store!.WallpaperImage, null,rootPath);
        }

        if (storeDto.SmallImage is not null)
        {
            smallImage = await fileServices.SaveFile(storeDto.SmallImage, EnImageType.Store,rootPath);
            DeleteStoreImage(null, user!.Store?.SmallImage,rootPath);
        }

        user!.Store!.SmallImage = smallImage ?? user!.Store!.SmallImage;
        user!.Store!.WallpaperImage = wallpaper ?? user!.Store!.WallpaperImage;
        user!.Store!.Name = storeDto.Name ?? user!.Store!.Name;
        user!.Store!.UpdatedAt = DateTime.Now;

        unitOfWork.StoreRepository.Update(user!.Store!);

        if ((storeDto.Longitude is null && storeDto.Latitude is not null) ||
            (storeDto.Longitude is not null && storeDto.Latitude is null))
        {
            fileServices.DeleteFile([wallpaper ?? "", smallImage ?? ""],rootPath);
            return new Result(false, "when update address you must change both longitude and latitude not one of them only", null, 400);
        }

        var result = await unitOfWork.SaveChanges();

        if (result < 1)
        {
            return new Result(false, "could not update store", null, 500);
        }

        var store = await unitOfWork.StoreRepository.GetStore(user.Store.Id);
        store?.Addresses = await unitOfWork.AddressRepository.GetAllAddressByOwnerId(store!.Id);

        await cache.RemoveByTagAsync(MemoryCacheKeys.StoresKey);

        return new Result(true, null, null, 204);
    }

    public async Task<Result> GetStorePage(Guid adminId, int storePerPage)
    {
        var store = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = store.IsValidateFunc();

        if (validationResult is not null)
            return new Result(false, validationResult.Item1, null, validationResult.Item2);

        var count = await unitOfWork.StoreRepository.GetStoresCount(storePerPage);
        return new Result(true, null, count, 200);
    }

    public async Task<Result> GetStoreByUserId(Guid userId)
    {
        var store = await unitOfWork.StoreRepository.GetStoreByUserId(userId);

        if (store is null)
            return new Result(false, "store not found", null, 404);

        var storeToDto = store.ToDto(config["url_file"] ?? "");
        return new Result(true, null, storeToDto, 200);
    }

    public async Task<Result> GetStoreByStoreId(Guid id)
    {
        var store = await unitOfWork.StoreRepository.GetStore(id);

        if (store is null)
            return new Result(false, "store not found", null, 404);

        var storeToDto = store.ToDto(config["url_file"] ?? "");
        return new Result(true, null, storeToDto, 200);
    }

    public async Task<Result> GetStores(Guid adminId, int pageNumber, int pageSize)
    {
        var user = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = user.IsValidateFunc(true);

        if (validationResult is not null)
        {
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var stores = await cache.GetOrCreateAsync(
            MemoryCacheKeys.StoresKey + "/" + adminId + '/' + pageNumber,
            async ct =>
            {
                var stores = (await unitOfWork.StoreRepository
                        .GetStores(pageNumber, pageSize))
                    .Select(st => st.ToDto(config["url_file"] ?? ""))
                    .ToList();
                return stores;
            },
            tags: [MemoryCacheKeys.StoresKey]);

        return new Result(true, null, stores, 200);
    }

    public async Task<Result> UpdateStoreStatus(
        Guid adminId,
         Guid storeId ,
         Action<StoreStatusDto>sendMessage)
    {
        var user = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = user.IsValidateFunc(false, isStore: true);

        if (validationResult is not null)
        {
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var store = await unitOfWork.StoreRepository.GetStore(storeId);

        validationResult = store!.user.IsValidateFunc(true);

        if (validationResult is null && store.UserId != user!.Id)
        {
            return new Result(false, "only Admin can update his store Status", null, 403);
        }

        store.IsBlock = !store.IsBlock;

        if (store.IsBlock && user?.IsUser == false)
        {
            return new Result(false, "this store is belong to admin you could not block it ", null, 403);
        }

        unitOfWork.StoreRepository.Update(store);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
            return new Result(false, "error while update store status", null, 500);

        var storeStatus = new StoreStatusDto
        {
            StoreId = storeId,
            Status = true
        };

        sendMessage.Invoke(storeStatus);
        await cache.RemoveByTagAsync(MemoryCacheKeys.StoresKey);

        return new Result(true, null, null, 204);
    }
}