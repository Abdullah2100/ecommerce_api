using api.application.Interface;
using api.application.Result;
using api.application.Services.Interface;
using api.domain.entity;
using api.Presentation.dto;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services;

public class PaymentTypeServices(
    IUnitOfWork unitOfWork,
    IFileServices fileServices,
    IConfiguration config
) : IPaymentTypeServices
{
    public async Task<IActionResult> Create(CreatePaymentTypeDto paymentTypeDto, Guid adminId)
    {
        var admin = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = admin.IsValidateFunc();
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var thumbnail = await fileServices.SaveFile(paymentTypeDto.Thumbnail, EnImageType.Payment);

        if (thumbnail is null)
        {
            return new ObjectResult("could not save payment image to api")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var paymentType = new PaymentType()
        {
            Id = ClsUtil.GenerateGuid(),
            IsHashCheckOperation = paymentTypeDto.IsHashCheckOperation,
            Name = paymentTypeDto.Name,
            Thumbnail = thumbnail,
            UserId = adminId
        };

        unitOfWork.PaymentTypeRepository.Add(paymentType);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("could not save payment type to system")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var paymentDto = paymentType.ToDto(config["url_file"]??"");

        return new ObjectResult(paymentDto)
            { StatusCode = StatusCodes.Status201Created };
    }

    public async Task<IActionResult> Update(UpdatePaymentTypeDto paymentTypeDto, Guid adminId)
    {
        var admin = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = admin.IsValidateFunc();
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var paymentType = await unitOfWork.PaymentTypeRepository.GetPaymentTypeGetPayment(paymentTypeDto.Id);

        if (paymentType is null)
        {
            return new ObjectResult("could not find payment type") { StatusCode = StatusCodes.Status404NotFound };
        }

        var isAlreadyExist = await unitOfWork
            .PaymentTypeRepository
            .IsExistPaymentType(paymentTypeDto!.Name!, paymentTypeDto.Id);

        if (isAlreadyExist)
        {
            return new ObjectResult("this payment type name is  already in use ")
                { StatusCode = StatusCodes.Status409Conflict };
        }


        var thumbnail = paymentTypeDto.Thumbnail == null
            ? null
            : await fileServices
                .SaveFile(paymentTypeDto.Thumbnail, EnImageType.Payment);

        paymentType.Name = paymentTypeDto.Name ?? paymentType.Name;
        paymentType.Thumbnail = thumbnail ?? paymentType.Thumbnail;
        paymentType.IsHashCheckOperation = paymentTypeDto.IsHashCheckOperation ?? paymentType.IsHashCheckOperation;

        unitOfWork.PaymentTypeRepository.Update(paymentType);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            fileServices.DeleteFile(thumbnail ?? "");

            return new ObjectResult("could not update payment type to system")
                { StatusCode = StatusCodes.Status409Conflict };
        }

        var paymentDto = paymentType.ToDto(config["url_file"]??"");

        return new ObjectResult(paymentDto)
            { StatusCode = StatusCodes.Status200OK };
    }

    public async Task<IActionResult> GetPaymentTypes(int pageNum, int pageSie = 25)
    {
        var paymentTypes = await unitOfWork.PaymentTypeRepository.GetPaymentTypes(pageNum, pageSie);

        var paymentTypesToDto = paymentTypes.Select(s => s.ToDto(config["url_file"]??"")).ToList();
        return new ObjectResult(paymentTypesToDto)
            { StatusCode = StatusCodes.Status200OK };

    }
}