using api.application;
using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using business.mapper;
using api.util;
using business.Services.Interface;
using data.dto.Request;
using data.util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace business.Services.Implement;

public class PaymentTypeServices(
    IUnitOfWork unitOfWork,
    IFileServices fileServices,
    IConfiguration config,
    HybridCache cache,
    ILogger<PaymentTypeServices> logger
) : IPaymentTypeServices
{
    public async Task<Result> Create(CreatePaymentTypeDto paymentTypeDto, Guid adminId,string rootPath)
    {
        logger.LogInformation("start creating paymentType");

        var admin = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = admin.IsValidateFunc();
        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", admin, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var thumbnail = await fileServices.SaveFile(paymentTypeDto.Thumbnail, EnImageType.Payment,rootPath);

        if (thumbnail is null)
        {
            logger.LogError("could not saved the payementType thumbnail to local");
            return new Result(false, "could not save payment image to api", null, 500);
        }

        var paymentType = new PaymentType
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
            return new Result(false, "could not save payment type to system", null, 500);
        }

        var paymentDto = paymentType.ToDto(config["url_file"] ?? "");

        await cache.RemoveByTagAsync(MemoryCacheKeys.PaymentTypesKey);

        logger.LogInformation("end creating paymentType");
        return new Result(true, null, paymentDto, 201);
    }

    public async Task<Result> Update(UpdatePaymentTypeDto paymentTypeDto, Guid adminId,string rootPath)
    {
        logger.LogInformation("start updating paymentType");

        var admin = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = admin.IsValidateFunc();
        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", adminId, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var paymentType = await unitOfWork.PaymentTypeRepository.GetPaymentTypeGetPayment(paymentTypeDto.Id);

        if (paymentType is null)
        {
            logger.LogError("payment is not found by {paymentId}", paymentTypeDto.Id);
            return new Result(false, "could not find payment type", null, 404);
        }

        var isAlreadyExist = await unitOfWork.PaymentTypeRepository.IsExistPaymentType(paymentTypeDto!.Name!, paymentTypeDto.Id);

        if (isAlreadyExist)
        {
            logger.LogError("this payment  {name} is  already in use", paymentTypeDto.Name);
            return new Result(false, "this payment type name is  already in use ", null, 409);
        }

        var thumbnail = paymentTypeDto.Thumbnail == null
            ? null
            : await fileServices.SaveFile(paymentTypeDto.Thumbnail, EnImageType.Payment,rootPath);

        paymentType.Name = paymentTypeDto.Name ?? paymentType.Name;
        paymentType.Thumbnail = thumbnail ?? paymentType.Thumbnail;
        paymentType.IsHashCheckOperation = paymentTypeDto.IsHashCheckOperation ?? paymentType.IsHashCheckOperation;

        unitOfWork.PaymentTypeRepository.Update(paymentType);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("could not updating payment type to system");
            fileServices.DeleteFile(thumbnail ?? "",rootPath);
            return new Result(false, "could not update payment type to system", null, 409);
        }

        var paymentDto = paymentType.ToDto(config["url_file"] ?? "");

        await cache.RemoveByTagAsync(MemoryCacheKeys.PaymentTypesKey);

        logger.LogInformation("end creating paymentType");
        return new Result(true, null, paymentDto, 200);
    }

    public async Task<Result> GetPaymentTypes(int pageNum, int pageSie = 25)
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
        return new Result(true, null, paymentTypes, 200);
    }
}