using api.application;
using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using business.mapper;
using api.util;
using business.Services.Interface;
using data.dto.Request;
using data.dto.Response;
using data.util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace business.Services.Implement;

public enum EnBelongToType
{
    Admin,
    Store
};

public class DeliveryServices(
    IConfiguration config,
    IUnitOfWork unitOfWork,
    IFileServices fileServices,
    IUserServices userServices,
    IAuthenticationService authenticationService,
    HybridCache cache,
    ILogger<DeliveryServices> logger
    )
    : IDeliveryServices
{
    public async Task<Result> Login(LoginDto loginDto)
    {
        logger.LogInformation("start login delivery");

        if (string.IsNullOrWhiteSpace(loginDto.DeviceToken))
        {
            logger.LogWarning("delivery login without device token");
            return new Result(false, "you should login from phone", null, 403);
        }

        var user = await unitOfWork.UserRepository.GetUser(loginDto.Username, ClsUtil.HashingText(loginDto.Password));
        var validationResult = user.IsValidateFunc(isAdmin: false);

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", user?.Id, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var delivery = await unitOfWork.DeliveryRepository.GetDeliveryByUserId(user!.Id);

        if (delivery is null)
        {
            logger.LogError("user {userId} is not linked to delivery", user.Id);
            return new Result(false, "delivery not found", null, 404);
        }

        if (delivery.IsBlocked)
        {
            logger.LogError("delivery {deliveryId} is blocked", delivery.Id);
            return new Result(false, "delivery is blocked", null, 403);
        }

        delivery.DeviceToken = loginDto.DeviceToken;

        unitOfWork.DeliveryRepository.Update(delivery);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("error from updating deviceToken for {deliveryId} in db", delivery.Id);
            return new Result(false, "error while adding delivery", null, 500);
        }

        var tokenData = await authenticationService.GenerateToken(
            id: user!.Id,
            email: user.Email,
            [EnUserType.Delivery]
        );

        logger.LogInformation("end login delivery");
        return new Result(true, null, tokenData, 200);
    }

    public async Task<Result> CreateDelivery(Guid userId, CreateDeliveryDto deliveryDto,string rootPath)
    {
        logger.LogInformation("start creating delivery");

        var user = await unitOfWork.UserRepository.GetUser(userId);
        var admin = user.IsValidateFunc();
        var store = user.IsValidateFunc(isAdmin: false, isStore: true);

        if ((admin is not null && user?.IsUser == false) || store != null)
        {
            logger.LogError("user not valid {userId} validationError {message}", userId, admin?.Item2 ?? store?.Item2);
            return new Result(false, admin?.Item1 ?? store?.Item1, null, admin?.Item2 ?? store?.Item2 ?? 400);
        }

        if (await unitOfWork.DeliveryRepository.IsExistByUserId(deliveryDto.UserId))
        {
            logger.LogError("user {userId} already linked to delivery", userId);
            return new Result(false, "delivery already exists", null, 409);
        }

        string? thumbnail = null;
        if (deliveryDto.Thumbnail is not null)
        {
            thumbnail = await fileServices.SaveFile(deliveryDto.Thumbnail, EnImageType.Delivery,rootPath);
        }

        var addressId = ClsUtil.GenerateGuid();
        var id = ClsUtil.GenerateGuid();
        var address = new Address
        {
            Id = addressId,
            Title = "my Place",
            CreatedAt = DateTime.Now,
            OwnerId = id
        };

        var delivery = new Delivery
        {
            Id = id,
            CreatedAt = DateTime.Now,
            UserId = deliveryDto.UserId,
            Thumbnail = thumbnail,
            Address = address,
            BelongTo = user?.Store?.Id ?? userId
        };

        unitOfWork.DeliveryRepository.Add(delivery);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            if (thumbnail != null)
                fileServices.DeleteFile(thumbnail,rootPath);
            logger.LogError("error from create delivery in db");
            return new Result(false, "error while adding delivery", null, 500);
        }

        delivery = await unitOfWork.DeliveryRepository.GetDelivery(id);
        var deliveryToDot = delivery?.ToDto(config["url_file"] ?? "");

        await cache.RemoveByTagAsync(MemoryCacheKeys.DeliveriesKey);

        logger.LogInformation("end  create delivery function");
        return new Result(true, null, deliveryToDot, 201);
    }

    public async Task<Result> UpdateDeliveryStatus(Guid id, bool status)
    {
        logger.LogInformation("start update delivery status  function");

        var delivery = await unitOfWork.DeliveryRepository.GetDelivery(id);

        if (delivery is null)
        {
            logger.LogError("delivery not exist by {userId}", id);
            return new Result(false, "delivery not found", null, 404);
        }

        if (delivery.IsBlocked)
        {
            logger.LogError("delivery {deliveryId} is blocked", delivery.Id);
            return new Result(false, "delivery is blocked", null, 403);
        }

        delivery.IsBlocked = status;

        unitOfWork.DeliveryRepository.Update(delivery);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("could not update delivery info in db");
            return new Result(false, "error while update delivery", null, 500);
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.DeliveriesKey);

        logger.LogInformation("end update delivery status  function");
        return new Result(true, null, null, 204);
    }

    public async Task<Result> GetDelivery(Guid id)
    {
        logger.LogInformation("start get delivery by id");

        var delivery = await unitOfWork.DeliveryRepository.GetDelivery(id);
        var validationResult = delivery.IsValidated();

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", id, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var deliveryDto = delivery?.ToDto(config["url_file"] ?? "");
        deliveryDto?.Analyse = await unitOfWork.DeliveryRepository.GetDeliveryAnalys(delivery!.Id!);
        logger.LogInformation("end get delivery by id");
        return new Result(true, null, deliveryDto, 200);
    }

    public async Task<Result> GetDeliveries(Guid belongToId, int pageNumber, int pageSize)
    {
        logger.LogInformation("start get deliveries by storeId per page");

        var user = await unitOfWork.UserRepository.GetUser(belongToId);
        var belongType = (user?.IsUser == false) switch
        {
            true => EnBelongToType.Admin,
            _ => EnBelongToType.Store
        };

        var id = Guid.NewGuid();
        switch (belongType)
        {
            case EnBelongToType.Store:
            {
                var validationResult = user.IsValidateFunc();
                if (validationResult is not null)
                {
                    logger.LogError("storeId {storeId} {ValidationResult}, ", belongToId, validationResult.Item2);
                    return new Result(false, validationResult.Item1, null, validationResult.Item2);
                }

                id = user!.Store!.Id!;
            }
            break;
            case EnBelongToType.Admin:
            {
                var validationResult = user.IsValidateFunc();
                if (validationResult is not null)
                {
                    logger.LogError("storeId {storeId} {ValidationResult}, ", belongToId, validationResult.Item2);
                    return new Result(false, validationResult.Item1, null, validationResult.Item2);
                }

                id = user!.Id;
            }
            break;
        }

        var deliveriesDto = await cache.GetOrCreateAsync(
            MemoryCacheKeys.DeliveriesKey + "/belong_to" + belongToId + '/' + pageNumber,
            async ct =>
            {
                var deliveriesDto = (await unitOfWork.DeliveryRepository.GetDeliveriesByBelongTo(id, pageNumber, pageSize))
                    ?.Select((de) => de.ToDto(config["url_file"] ?? ""))
                    .ToList();

                if (deliveriesDto == null) return null;
                foreach (var delivery in deliveriesDto)
                {
                    delivery.Analyse = await unitOfWork.DeliveryRepository.GetDeliveryAnalys(delivery.Id);
                }

                return new List<DeliveryDto>();
            },
            tags: [MemoryCacheKeys.DeliveriesKey]);

        logger.LogInformation("end get deliveries by storeId per page");
        return new Result(true, null, deliveriesDto, 200);
    }

    public async Task<Result> UpdateDelivery(UpdateDeliveryDto deliveryDto, Guid id,string rootPath)
    {
        logger.LogInformation("start update delivery info  function");

        var delivery = await unitOfWork.DeliveryRepository.GetDelivery(id);
        var validationResult = delivery.IsValidated();

        if (validationResult is not null)
        {
            logger.LogError("delivery not valid {deliveryId} validationError {message}", id, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        if (deliveryDto.Longitude is not null && deliveryDto.Latitude is not null)
        {
            var addressHolder = delivery?.Address ?? new Address
            {
                Id = ClsUtil.GenerateGuid(),
                CreatedAt = DateTime.Now,
                OwnerId = delivery!.Id,
                IsCurrent = true,
                Title = "My Place"
            };
            addressHolder.Longitude = deliveryDto.Longitude;
            addressHolder.Latitude = deliveryDto.Latitude;
            addressHolder.IsCurrent = true;
            if (delivery.Address is null)
                unitOfWork.AddressRepository.Add(addressHolder);
            else
                unitOfWork.AddressRepository.Update(addressHolder);
        }

        var userUpdateData = new UpdateUserInfoDto
        {
            Name = deliveryDto.Name,
            Phone = deliveryDto.Phone,
            Thumbnail = deliveryDto.Thumbnail,
            Password = deliveryDto.Password,
            NewPassword = deliveryDto.NewPassword,
        };

        if (deliveryDto.Thumbnail is not null)
        {
            var previous = delivery?.Thumbnail;
            if (previous is not null)
                fileServices.DeleteFile(previous,rootPath);

            string? newThumbNail = null;
            newThumbNail = await fileServices.SaveFile(deliveryDto.Thumbnail, EnImageType.Delivery,rootPath);
            delivery?.Thumbnail = newThumbNail;
            unitOfWork.DeliveryRepository.Update(delivery!);
        }

        if (userUpdateData.IsUpdateAnyFeild() is true)
        {
            await userServices.UpdateUser(userUpdateData, delivery!.UserId, rootPath,true);
        }

        var result = await unitOfWork.SaveChanges();
        await cache.RemoveByTagAsync(MemoryCacheKeys.DeliveriesKey);

        logger.LogInformation("end update delivery status  function");
        return result < 1
            ? new Result(false, "Something went wrong", null, 500)
            : new Result(true, null, null, 204);
    }
}