using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.shared.mapper;
using api.util;
using ecommerce_api.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace api.application.Services.Implement;

public class VariantServices(IUnitOfWork unitOfWork, IMemoryCache memoryCache)
    : IVariantServices
{
    public async Task<IActionResult> CreateVariant(
        CreateVariantDto variantDto,
        Guid adminId
    )
    {
        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc(true);
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        if (await unitOfWork.VariantRepository.IsExist(variantDto.Name))
        {
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
            return new ObjectResult("error while adding new variant")
            { StatusCode = StatusCodes.Status404NotFound };
        }

        var variantToDto = variant?.ToDto();

        memoryCache.Remove(MemoryCachKeys.VairantsKey);

        return new ObjectResult(variantToDto)
        { StatusCode = StatusCodes.Status201Created };
    }

    public async Task<IActionResult> UpdateVariant(
        UpdateVariantDto variantDto,
        Guid adminId
    )
    {
        if (variantDto.IsEmpty())
            return new ObjectResult("No Found Update Chanage")
            { StatusCode = StatusCodes.Status400BadRequest };


        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc(false, isStore: true);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var variant = await unitOfWork.VariantRepository.GetVarient(variantDto.Id);

        if (variant is null)
        {
            return new ObjectResult("variant not found")
            { StatusCode = StatusCodes.Status404NotFound };
        }

        if (variantDto.Name is not null)
            if (await unitOfWork.VariantRepository.IsExist(variantDto.Name, variantDto.Id))
            {
                return new ObjectResult("name of variant already exist")
                { StatusCode = StatusCodes.Status409Conflict };
            }


        variant.Name = variantDto.Name ?? variant.Name;

        unitOfWork.VariantRepository.Update(variant);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while update variant")
            { StatusCode = StatusCodes.Status500InternalServerError };
        }

        memoryCache.Remove(MemoryCachKeys.VairantsKey);

        return new ObjectResult(null)
        { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> DeleteVariant(Guid vairantId, Guid adminId)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc(false, isStore: true);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }


        var variant = await unitOfWork.VariantRepository.GetVarient(vairantId);

        if (variant is null)
        {
            return new ObjectResult("variant not found")
            { StatusCode = StatusCodes.Status404NotFound };
        }


        unitOfWork.VariantRepository
            .Delete(vairantId);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while delete variant")
            { StatusCode = StatusCodes.Status500InternalServerError };
        }

        memoryCache.Remove(MemoryCachKeys.VairantsKey);

        return new ObjectResult(null)
        { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> GetVariantPage(Guid adminId, int variantPerPage)

    {
        var user = await unitOfWork.UserRepository.GetUser(adminId);

        var validationResult = user.IsValidateFunc(false, isStore: true);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }


        var count = await unitOfWork.VariantRepository.GetVarientCount(variantPerPage);


        return new ObjectResult(count)
        { StatusCode = StatusCodes.Status200OK };
    }


    public async Task<IActionResult> GetVariants(int page, int pageSize)
    {

        var varants = memoryCache.GetOrCreate(MemoryCachKeys.VairantsKey, async entry =>
        {
            entry.Size = 1;
            return (await unitOfWork.VariantRepository
            .GetVarients(page, pageSize))
            .Select(va => va.ToDto()).ToList(); ;
        });

        return new ObjectResult(varants)
        { StatusCode = StatusCodes.Status200OK };
    }
}