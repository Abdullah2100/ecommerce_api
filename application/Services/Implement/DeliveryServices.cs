using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services.Implement;

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
    IAuthenticationService authenticationService
)
    : IDeliveryServices
{
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.DeviceToken))
            return new ObjectResult("you should login from phone") { StatusCode = StatusCodes.Status403Forbidden };


        var user = await unitOfWork.UserRepository
            .GetUser(
                loginDto.Username,
                ClsUtil.HashingText(loginDto.Password));


        var validationResult = user.IsValidateFunc(isAdmin: false);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var delivery = await unitOfWork.DeliveryRepository.GetDeliveryByUserId(user!.Id);

        if (delivery is null)
        {
            return new ObjectResult("delivery not found") { StatusCode = StatusCodes.Status404NotFound };
        }


        if (delivery.IsBlocked)
            return new ObjectResult("delivery is blocked") { StatusCode = StatusCodes.Status403Forbidden };

        delivery.DeviceToken = loginDto.DeviceToken;

        unitOfWork.DeliveryRepository.Update(delivery);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while adding delivery")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }


        var tokenData = await authenticationService.GenerateToken(
            id: user!.Id,
            email: user.Email,
            [EnUserType.Delivery]
        );


        return new ObjectResult(tokenData)
            { StatusCode = StatusCodes.Status200OK };
    }


    public async Task<IActionResult> CreateDelivery(
        Guid userId,
        CreateDeliveryDto deliveryDto
    )
    {
        User? user = await unitOfWork.UserRepository
            .GetUser(userId);


        var admin = user.IsValidateFunc();
        var store = user.IsValidateFunc(isAdmin: false, isStore: true);


        if ((admin is not null && user?.IsUser == false) || store != null)
        {
            return new ObjectResult(admin?.Item1 ?? store?.Item1) { StatusCode = admin?.Item2 ?? store?.Item2 };
        }


        if (await unitOfWork.DeliveryRepository.IsExistByUserId(deliveryDto.UserId))
        {
            return new ObjectResult("delivery already exists") { StatusCode = StatusCodes.Status409Conflict };
        }


        string? thumbnail = null;
        if (deliveryDto.Thumbnail is not null)
        {
            thumbnail = await fileServices
                .SaveFile(
                    deliveryDto.Thumbnail,
                    EnImageType.Delivery);
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
                fileServices.DeleteFile(thumbnail);
            return new ObjectResult("error while adding delivery")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        delivery = await unitOfWork.DeliveryRepository.GetDelivery(id);


        var deliveryToDot = delivery?.ToDto(config["url_file"]??"");

        return new ObjectResult(deliveryToDot)
            { StatusCode = StatusCodes.Status201Created };
    }

    public async Task<IActionResult> UpdateDeliveryStatus(Guid id, bool status)
    {
        var delivery = await unitOfWork.DeliveryRepository
            .GetDelivery(id);
        if (delivery is null)
        {
            return new ObjectResult("delivery not found")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        if (delivery.IsBlocked)
        {
            return new ObjectResult("delivery is blocked")
                { StatusCode = StatusCodes.Status403Forbidden };
        }

        delivery.IsBlocked = status;

        unitOfWork.DeliveryRepository.Update(delivery);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while update delivery")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }


    public async Task<IActionResult> GetDelivery(Guid id)
    {
        var delivery = await unitOfWork.DeliveryRepository
            .GetDelivery(id);

        var validationResult = delivery.IsValidated();

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var deliveryDto = delivery?.ToDto(config["url_file"]??"");
        deliveryDto?.Analyse = await unitOfWork.DeliveryRepository.GetDeliveryAnalys(delivery!.Id!);

        return new ObjectResult(deliveryDto)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }


    public async Task<IActionResult> GetDeliveries(
        Guid belongToId,
        int pageNumber,
        int pageSize
    )
    {
        var user = await unitOfWork.UserRepository
            .GetUser(belongToId);

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
                var validationResult = user.IsValidateFunc(isStore: true);
                if (validationResult is not null)
                {
                    return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
                }

                id = user!.Store!.Id!;
            }
                break;
            case EnBelongToType.Admin:
            {
                var validationResult = user.IsValidateFunc();
                if (validationResult is not null)
                {
                    return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
                }

                id = user!.Id;
            }
                break;
        }


        var deliveryDto = (await unitOfWork.DeliveryRepository
                .GetDeliveriesByBelongTo(id, pageNumber, pageSize))
            ?.Select((de) => de.ToDto(config["url_file"]??""))
            .ToList();

        if (deliveryDto is null) return new ObjectResult(deliveryDto) { StatusCode = StatusCodes.Status200OK };

        foreach (var delivery in deliveryDto)
        {
            delivery.Analyse = await unitOfWork.DeliveryRepository.GetDeliveryAnalys(delivery.Id);
        }


        return new ObjectResult(deliveryDto) { StatusCode = StatusCodes.Status200OK };
    }


    public async Task<IActionResult> UpdateDelivery(UpdateDeliveryDto deliveryDto, Guid id)
    {
        var delivery = await unitOfWork.DeliveryRepository
            .GetDelivery(id);

        var validationResult = delivery.IsValidated();

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }


        if (deliveryDto.Longitude is not null && deliveryDto.Latitude is not null)
        {
            var addressHolder = delivery?.Address ?? new Address()
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
                fileServices.DeleteFile(filePath: previous);

            string? newThumbNail = null;
            newThumbNail = await fileServices.SaveFile(file: deliveryDto.Thumbnail, type: EnImageType.Delivery);
            delivery?.Thumbnail = newThumbNail;

            unitOfWork.DeliveryRepository.Update(delivery!);
        }

        if (userUpdateData.IsUpdateAnyFeild() is true)
        {
            await userServices.UpdateUser(userUpdateData, delivery!.UserId, true);
        }

        var result = await unitOfWork.SaveChanges();


        return result < 1
            ? new ObjectResult("Something went wrong") { StatusCode = StatusCodes.Status500InternalServerError }
            : new ObjectResult(null) { StatusCode = StatusCodes.Status204NoContent };
    }
}