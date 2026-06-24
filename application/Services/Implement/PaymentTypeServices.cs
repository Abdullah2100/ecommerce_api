using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;

namespace api.application.Services.Implement;

public class PaymentTypeServices(
    IUnitOfWork unitOfWork,
    IFileServices fileServices,
    IConfiguration config,
    HybridCache cache,
    ILogger<PaymentTypeServices> logger
) : IPaymentTypeServices
{

    public async Task<IActionResult> Create(CreatePaymentTypeDto paymentTypeDto, Guid adminId)
    {
        logger.LogInformation("start creating paymentType");

        var admin = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = admin.IsValidateFunc();
        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", admin, validationResult.Item2);

            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var thumbnail = await fileServices.SaveFile(paymentTypeDto.Thumbnail, EnImageType.Payment);

        if (thumbnail is null)
        {

            logger.LogError("could not saved the payementType thumbnail to local");

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
            logger.LogError("could not save payment type to system");

            return new ObjectResult("could not save payment type to system")
            { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var paymentDto = paymentType.ToDto(config["url_file"] ?? "");

        await cache.RemoveByTagAsync(MemoryCacheKeys.PaymentTypesKey);

        logger.LogInformation("end creating paymentType");

        return new ObjectResult(paymentDto)
        { StatusCode = StatusCodes.Status201Created };
    }


    public async Task<IActionResult> Update(UpdatePaymentTypeDto paymentTypeDto, Guid adminId)
    {
        logger.LogInformation("start updating paymentType");

        var admin = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = admin.IsValidateFunc();
        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", adminId, validationResult.Item2);

            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var paymentType = await unitOfWork.PaymentTypeRepository.GetPaymentTypeGetPayment(paymentTypeDto.Id);

        if (paymentType is null)
        {
            logger.LogError("payment is not found by {paymentId}", paymentTypeDto.Id);

            return new ObjectResult("could not find payment type") { StatusCode = StatusCodes.Status404NotFound };
        }

        var isAlreadyExist = await unitOfWork
            .PaymentTypeRepository
            .IsExistPaymentType(paymentTypeDto!.Name!, paymentTypeDto.Id);

        if (isAlreadyExist)
        {
            logger.LogError("this payment  {name} is  already in use", paymentTypeDto.Name);

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
            logger.LogError("could not updating payment type to system");

            fileServices.DeleteFile(thumbnail ?? "");

            return new ObjectResult("could not update payment type to system")
            { StatusCode = StatusCodes.Status409Conflict };
        }

        var paymentDto = paymentType.ToDto(config["url_file"] ?? "");

        await cache.RemoveByTagAsync(MemoryCacheKeys.PaymentTypesKey);

        logger.LogInformation("end creating paymentType");

        return new ObjectResult(paymentDto)
        { StatusCode = StatusCodes.Status200OK };
    }


    public async Task<IActionResult> GetPaymentTypes(int pageNum, int pageSie = 25)
    {
        logger.LogInformation("start getting paymentTypes page by page");

        var paymentTypes = await cache.GetOrCreateAsync(
            MemoryCacheKeys.PaymentTypesKey + '/' + pageNum,
            async ct =>
            {
                var paymentTypes = (await unitOfWork.PaymentTypeRepository.GetPaymentTypes(pageNum, pageSie))
                    .Select(s => s.ToDto(config["url_file"] ?? ""));
                return paymentTypes;
            },
            tags: [MemoryCacheKeys.PaymentTypesKey]);

        logger.LogInformation("end getting paymentTypes page by page");

        return new ObjectResult(paymentTypes)
        { StatusCode = StatusCodes.Status200OK };
    }



}