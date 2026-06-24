using System.Text.Json;
using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using Sats.PostgresDistributedCache;

namespace api.application.Services.Implement;

public class VariantServices(
    IUnitOfWork unitOfWork,
    HybridCache memoryCache,
    ILogger<VariantServices> logger)
    : IVariantServices
{
    public async Task<IActionResult> CreateVariant(
        CreateVariantDto variantDto,
        Guid adminId
    )
    {
        logger.LogInformation("start creating variant");
        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc(true);
        if (validationResult is not null)
        {
            logger.LogError("admin not valid {adminId} validationError {message}", adminId, validationResult.Item2);

            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        if (await unitOfWork.VariantRepository.IsExist(variantDto.Name))
        {
            logger.LogError("already exists variant by name {nameVariant}", variantDto.Name);

            return new ObjectResult("there are variant with the same name")
            { StatusCode = StatusCodes.Status404NotFound };
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

            return new ObjectResult("error while adding new variant")
            { StatusCode = StatusCodes.Status404NotFound };
        }

        var variantToDto = variant?.ToDto();

        await memoryCache.RemoveByTagAsync(MemoryCacheKeys.VariantsKey);

        logger.LogInformation("end creating variant");

        return new ObjectResult(variantToDto)
        { StatusCode = StatusCodes.Status201Created };
    }

    public async Task<IActionResult> UpdateVariant(
        UpdateVariantDto variantDto,
        Guid adminId
    )
    {
        logger.LogInformation("start updating  variant");

        if (variantDto.IsEmpty())
        {
            logger.LogError("no change found at the variant object from admin ");

            return new ObjectResult("No Found Update Chanage")
            { StatusCode = StatusCodes.Status400BadRequest };
        }

        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc(false, isStore: true);

        if (validationResult is not null)
        {
            logger.LogError("admin not valid {adminId} validationError {message}", adminId, validationResult.Item2);

            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var variant = await unitOfWork.VariantRepository.GetVarient(variantDto.Id);

        if (variant is null)
        {
            logger.LogError("anot found variant by {id}", variantDto.Id);

            return new ObjectResult("variant not found")
            { StatusCode = StatusCodes.Status404NotFound };
        }

        if (variantDto.Name is not null)
            if (await unitOfWork.VariantRepository.IsExist(variantDto.Name, variantDto.Id))
            {
                logger.LogError("already exists variant by name {nameVariant}", variantDto.Name);

                return new ObjectResult("name of variant already exist")
                { StatusCode = StatusCodes.Status409Conflict };
            }


        variant.Name = variantDto.Name ?? variant.Name;

        unitOfWork.VariantRepository.Update(variant);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("errore while updating  variant");

            return new ObjectResult("error while update variant")
            { StatusCode = StatusCodes.Status500InternalServerError };
        }

        await memoryCache.RemoveByTagAsync(MemoryCacheKeys.VariantsKey);

        logger.LogInformation("end updating  variant");

        return new ObjectResult(null)
        { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> DeleteVariant(Guid vairantId, Guid adminId)
    {
        logger.LogInformation("start deleting  variant");

        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc(false, isStore: true);

        if (validationResult is not null)
        {
            logger.LogError("admin not valid {adminId} validationError {message}", adminId, validationResult.Item2);

            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }


        var variant = await unitOfWork.VariantRepository.GetVarient(vairantId);

        if (variant is null)
        {
            logger.LogError("anot found variant by {id}", vairantId);

            return new ObjectResult("variant not found")
            { StatusCode = StatusCodes.Status404NotFound };
        }


        unitOfWork.VariantRepository
            .Delete(vairantId);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("errore while deleting  variant");

            return new ObjectResult("error while delete variant")
            { StatusCode = StatusCodes.Status500InternalServerError };
        }

        await memoryCache.RemoveByTagAsync(MemoryCacheKeys.VariantsKey);

        logger.LogInformation("end deleting  variant");

        return new ObjectResult(null)
        { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> GetVariantPage(Guid userId, int variantPerPage)

    {
        logger.LogInformation("start getting variants page count ");

        var user = await unitOfWork.UserRepository.GetUser(userId);

        var validationResult = user.IsValidateFunc(false, isStore: true);

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", userId, validationResult.Item2);

            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }


        var count = await unitOfWork.VariantRepository.GetVarientCount(variantPerPage);

        logger.LogInformation("end getting variants page count");

        return new ObjectResult(count)
        { StatusCode = StatusCodes.Status200OK };
    }


    public async Task<IActionResult> GetVariants(int page, int pageSize)
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

        return new ObjectResult(variants)
        { StatusCode = StatusCodes.Status200OK };
    }
}