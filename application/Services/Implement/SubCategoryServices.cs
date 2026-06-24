using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;

namespace api.application.Services.Implement;

public class SubCategoryServices(
    IUnitOfWork unitOfWork,
    HybridCache cache,
    ILogger<SubCategoryServices> logger)
    : ISubCategoryServices
{
    public async Task<IActionResult> CreateSubCategory(
        Guid storeId,
        CreateSubCategoryDto subCategoryDto
    )
    {

        logger.LogInformation("start creat subCateogry");

        var store = await unitOfWork.StoreRepository
            .GetStore(storeId);


        if (store is not null)
        {
            logger.LogError("not found  store by {storeId}", storeId);

            return new ObjectResult("Store Not Found")
            { StatusCode = StatusCodes.Status404NotFound };
        }


        var count = await unitOfWork.SubCategoryRepository.GetSubCategoriesCount(storeId);

        if (count == 20)
        {
            logger.LogError("store {storeId} is hit the maximum of 20 subCategories for one store ", storeId);

            return new ObjectResult("store can maximum 20 subcategories")
            { StatusCode = StatusCodes.Status403Forbidden };
        }

        var id = ClsUtil.GenerateGuid();

        var subCategory = new SubCategory
        {
            Id = id,
            CategoryId = subCategoryDto.CategoryId,
            StoreId = storeId,
            Name = subCategoryDto.Name,
            UpdatedAt = null,
            CreatedAt = DateTime.Now,
        };

        unitOfWork.SubCategoryRepository.Add(subCategory);

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("errore while creating subCatogry");

            return new ObjectResult("error while adding new subcategory")
            { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var subCategoryToDto = subCategory.ToDto();

        await cache.RemoveByTagAsync(MemoryCacheKeys.StoreSubCategoriesKey);

        logger.LogInformation("start creat subCateogry");

        return new ObjectResult(subCategoryToDto)
        { StatusCode = StatusCodes.Status201Created };
    }

    public async Task<IActionResult> UpdateSubCategory(
        Guid storeId,
        UpdateSubCategoryDto subCategoryDto
    )
    {
        logger.LogInformation("start updating subCateogry");

        if (subCategoryDto.IsEmpty())
            return new ObjectResult("No Change Found At Data")
            { StatusCode = StatusCodes.Status400BadRequest };


        var store = await unitOfWork.StoreRepository
            .GetStore(storeId);


        if (store is not null)
        {
            logger.LogError("not found user store by {storeId}", storeId);

            return new ObjectResult("Store Not Found")
            { StatusCode = StatusCodes.Status404NotFound };
        }


        var subCategory = await unitOfWork.SubCategoryRepository
            .GetSubCategory(subCategoryDto.Id);

        if (subCategory is null || subCategory.StoreId != storeId)
        {
            logger.LogError("subCategory {subCategoryId} not found", subCategoryDto.Id);

            return new ObjectResult("subcategory not found")
            { StatusCode = StatusCodes.Status404NotFound };
        }

        if (subCategoryDto.CategoryId is not null &&
            !await unitOfWork.CategoryRepository.IsExist((Guid)subCategoryDto.CategoryId)
           )
        {
            logger.LogError("subCategory {subCategoryId} is invalid", subCategoryDto.Id);

            return new ObjectResult("invalid category")
            { StatusCode = StatusCodes.Status404NotFound };
        }

        subCategory.Name = subCategoryDto.Name ?? subCategory.Name;
        subCategory.CategoryId = subCategoryDto.CategoryId ?? subCategory.CategoryId;
        subCategory.UpdatedAt = DateTime.Now;

        unitOfWork.SubCategoryRepository.Update(subCategory);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("errore while updating subCatogry");

            return new ObjectResult("error while update subcategory")
            { StatusCode = StatusCodes.Status404NotFound };
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.StoreSubCategoriesKey);

        logger.LogInformation("end updating subCateogry");

        return new ObjectResult(null)
        { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> DeleteSubCategory(Guid id, Guid storeId)
    {
        logger.LogInformation("start delete subCateogry");

        var store = await unitOfWork.StoreRepository
            .GetStore(storeId);


        if (store is not null)
        {
            logger.LogError("not found  store by {storeId}", storeId);

            return new ObjectResult("Store Not Found")
            { StatusCode = StatusCodes.Status404NotFound };
        }

        var subCategory = await unitOfWork.SubCategoryRepository
            .GetSubCategory(id);

        if (subCategory is null)
        {

            logger.LogError("not found  subCateogry by {subCateogryId}", subCategory?.Id);

            return new ObjectResult("SubCategory Not Found")
            { StatusCode = StatusCodes.Status404NotFound };
        }

        if (subCategory.StoreId != storeId)
        {
            logger.LogError("subCateogry {subCateogryId} is not belong to {storeId}", subCategory?.Id, storeId);

            return new ObjectResult("the SubCategory does not belong to this store")
            { StatusCode = StatusCodes.Status403Forbidden };
        }

        unitOfWork.SubCategoryRepository.Delete(id);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("error while deleting subcategory");

            return new ObjectResult("error while deleting subcategory")
            { StatusCode = StatusCodes.Status403Forbidden };
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.StoreSubCategoriesKey);

        logger.LogInformation("end delete subCateogry");

        return new ObjectResult(null)
        { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> GetSubCategories(Guid storeId, int page, int length)
    {
        logger.LogInformation("start getting subCateogry page by page by storeId");

        var subCategories = await cache.GetOrCreateAsync(
            MemoryCacheKeys.StoreSubCategoriesKey + '/' + storeId + '/' + page,
            async ct =>
            {
                var subCategories = (await unitOfWork.SubCategoryRepository
                        .GetSubCategories(storeId, page, length))
                    .Select(su => su.ToDto())
                    .ToList();
                return subCategories;
            },
            tags: [MemoryCacheKeys.StoreSubCategoriesKey]);


        logger.LogInformation("end getting subCateogry page by page by storeId");

        return new ObjectResult(subCategories)
        { StatusCode = StatusCodes.Status200OK };
    }

    public async Task<IActionResult> GetSubCategoryAll(
        Guid adminId,
        int page,
        int length)
    {
        logger.LogInformation("start getting subCateogry page by page by adminId");

        var user = await unitOfWork.UserRepository
            .GetUser(adminId);
        var validationResult = user.IsValidateFunc(isAdmin: true);

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", user?.Id, validationResult.Item2);

            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var subcategories = await cache.GetOrCreateAsync(
            MemoryCacheKeys.StoreSubCategoriesKey + "/admin/" + adminId + '/' + page,
            async ct =>
            {
                var subcategories = (await unitOfWork.SubCategoryRepository
                        .GetSubCategories(page, length))
                    .Select(ba => ba.ToDto())
                    .ToList();
                return subcategories;
            },
            tags: [MemoryCacheKeys.StoreSubCategoriesKey]);

        logger.LogInformation("end getting subCateogry page by page by adminId");
        return new ObjectResult(subcategories)
        { StatusCode = StatusCodes.Status200OK };
    }
}