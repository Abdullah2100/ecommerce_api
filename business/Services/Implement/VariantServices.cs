using System.Text.Json;
using api.application;
using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using business.mapper;
using api.util;
using data.dto.Request;
using data.util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace business.Services.Implement;

public class VariantServices(
    IUnitOfWork unitOfWork,
    HybridCache memoryCache,
    ILogger<VariantServices> logger)
    : IVariantServices
{
    public async Task<Result> CreateVariant(
        CreateVariantDto variantDto,
        Guid adminId
    )
    {
        logger.LogInformation("start creating variant");
        var user = await unitOfWork.UserRepository.GetUser(adminId);

        var validationResult = user.IsValidateFunc(true);
        if (validationResult is not null)
        {
            logger.LogError("admin not valid {adminId} validationError {message}", adminId, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        if (await unitOfWork.VariantRepository.IsExist(variantDto.Name))
        {
            logger.LogError("already exists variant by name {nameVariant}", variantDto.Name);
            return new Result(false, "there are variant with the same name", null, 404);
        }

        var id = ClsUtil.GenerateGuid();

        var variant = new Variant
        {
            Id = id,
            Name = variantDto.Name
        };

        unitOfWork.VariantRepository.Add(variant);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("errore while creating new variant");
            return new Result(false, "error while adding new variant", null, 404);
        }

        var variantToDto = variant?.ToDto();
        await memoryCache.RemoveByTagAsync(MemoryCacheKeys.VariantsKey);

        logger.LogInformation("end creating variant");
        return new Result(true, null, variantToDto, 201);
    }

    public async Task<Result> UpdateVariant(
        UpdateVariantDto variantDto,
        Guid adminId
    )
    {
        logger.LogInformation("start updating  variant");

        if (variantDto.IsEmpty())
        {
            logger.LogError("no change found at the variant object from admin ");
            return new Result(false, "No Found Update Chanage", null, 400);
        }

        var user = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = user.IsValidateFunc(false, isStore: true);

        if (validationResult is not null)
        {
            logger.LogError("admin not valid {adminId} validationError {message}", adminId, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var variant = await unitOfWork.VariantRepository.GetVarient(variantDto.Id);

        if (variant is null)
        {
            logger.LogError("anot found variant by {id}", variantDto.Id);
            return new Result(false, "variant not found", null, 404);
        }

        if (variantDto.Name is not null)
            if (await unitOfWork.VariantRepository.IsExist(variantDto.Name, variantDto.Id))
            {
                logger.LogError("already exists variant by name {nameVariant}", variantDto.Name);
                return new Result(false, "name of variant already exist", null, 409);
            }

        variant.Name = variantDto.Name ?? variant.Name;

        unitOfWork.VariantRepository.Update(variant);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("errore while updating  variant");
            return new Result(false, "error while update variant", null, 500);
        }

        await memoryCache.RemoveByTagAsync(MemoryCacheKeys.VariantsKey);

        logger.LogInformation("end updating  variant");
        return new Result(true, null, null, 204);
    }

    public async Task<Result> DeleteVariant(Guid vairantId, Guid adminId)
    {
        logger.LogInformation("start deleting  variant");

        var user = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = user.IsValidateFunc(false, isStore: true);

        if (validationResult is not null)
        {
            logger.LogError("admin not valid {adminId} validationError {message}", adminId, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var variant = await unitOfWork.VariantRepository.GetVarient(vairantId);

        if (variant is null)
        {
            logger.LogError("anot found variant by {id}", vairantId);
            return new Result(false, "variant not found", null, 404);
        }

        unitOfWork.VariantRepository.Delete(vairantId);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("errore while deleting  variant");
            return new Result(false, "error while delete variant", null, 500);
        }

        await memoryCache.RemoveByTagAsync(MemoryCacheKeys.VariantsKey);

        logger.LogInformation("end deleting  variant");
        return new Result(true, null, null, 204);
    }

    public async Task<Result> GetVariantPage(Guid userId, int variantPerPage)
    {
        logger.LogInformation("start getting variants page count ");

        var user = await unitOfWork.UserRepository.GetUser(userId);
        var validationResult = user.IsValidateFunc(false, isStore: true);

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", userId, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var count = await unitOfWork.VariantRepository.GetVarientCount(variantPerPage);

        logger.LogInformation("end getting variants page count");
        return new Result(true, null, count, 200);
    }

    public async Task<Result> GetVariants(int page, int pageSize)
    {
        logger.LogInformation("start getting variants page by page ");

        var variants = await memoryCache.GetOrCreateAsync(MemoryCacheKeys.VariantsKey + page,
            async dt =>
            {
                var variants = (await unitOfWork.VariantRepository
                        .GetVarients(page, pageSize))
                    .Select(va => va.ToDto()).ToList();

                return variants;
            },
            tags: [MemoryCacheKeys.VariantsKey]);

        logger.LogInformation("end getting variants page by page ");
        return new Result(true, null, variants, 200);
    }
}