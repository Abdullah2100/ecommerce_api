using api.application;
using api.application.Services.Interface;
using api.domain.entity;
using data.mapper;
using api.util;
using data;
using data.dto.Request;
using data.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace business.Services.Implement;

public class SubCategoryServices(
    IUnitOfWork unitOfWork,
    HybridCache cache,
    ILogger<SubCategoryServices> logger)
    : ISubCategoryServices
{
    public async Task<Result> CreateSubCategory(
        Guid storeId,
        CreateSubCategoryDto subCategoryDto
    )
    {
        logger.LogInformation("start creat subCategory");

        var store = await unitOfWork.StoreRepository.GetStore(storeId, false);

        if (store is not null)
        {
            logger.LogError("not found  store by {storeId}", storeId);
            return new Result(false, "Store Not Found", null, 404);
        }

        var count = await unitOfWork.SubCategoryRepository.GetSubCategoriesCount(storeId);

        if (count == 20)
        {
            logger.LogError("store {storeId} is hit the maximum of 20 subCategories for one store ", storeId);
            return new Result(false, "store can maximum 20 subcategories", null, 403);
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

        await unitOfWork.SubCategoryRepository.Add(subCategory);

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("error while creating subCategory");
            return new Result(false, "error while adding new subcategory", null, 500);
        }

        var subCategoryToDto = subCategory.ToDto();

        await cache.RemoveByTagAsync(MemoryCacheKeys.StoreSubCategoriesKey);

        logger.LogInformation("start creat subCategory");

        return new Result(true, null, subCategoryToDto, 201);
    }

    public async Task<Result> UpdateSubCategory(
        Guid storeId,
        UpdateSubCategoryDto subCategoryDto
    )
    {
        logger.LogInformation("start updating subCateogry");

        if (subCategoryDto.IsEmpty())
            return new Result(false, "No Change Found At Data", null, 400);

        var store = await unitOfWork.StoreRepository.GetStore(storeId, false);

        if (store is not null)
        {
            logger.LogError("not found user store by {storeId}", storeId);
            return new Result(false, "Store Not Found", null, 404);
        }

        var subCategory = await unitOfWork.SubCategoryRepository.GetSubCategory(subCategoryDto.Id);

        if (subCategory is null || subCategory.StoreId != storeId)
        {
            logger.LogError("subCategory {subCategoryId} not found", subCategoryDto.Id);
            return new Result(false, "subcategory not found", null, 404);
        }

        if (subCategoryDto.CategoryId is not null &&
            !await unitOfWork.CategoryRepository.IsExist((Guid)subCategoryDto.CategoryId)
           )
        {
            logger.LogError("subCategory {subCategoryId} is invalid", subCategoryDto.Id);
            return new Result(false, "invalid category", null, 404);
        }

        subCategory.Name = subCategoryDto.Name ?? subCategory.Name;
        subCategory.CategoryId = subCategoryDto.CategoryId ?? subCategory.CategoryId;
        subCategory.UpdatedAt = DateTime.Now;

        unitOfWork.SubCategoryRepository.Update(subCategory);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("error while updating subCategory");
            return new Result(false, "error while update subcategory", null, 404);
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.StoreSubCategoriesKey);

        logger.LogInformation("end updating subCategory");

        return new Result(true, null, null, 204);
    }

    public async Task<Result> DeleteSubCategory(Guid id, Guid storeId)
    {
        logger.LogInformation("start delete subCategory");

        var store = await unitOfWork.StoreRepository.GetStore(storeId, false);

        if (store is not null)
        {
            logger.LogError("not found  store by {storeId}", storeId);
            return new Result(false, "Store Not Found", null, 404);
        }

        var subCategory = await unitOfWork.SubCategoryRepository.GetSubCategory(id);

        if (subCategory is null)
        {
            logger.LogError("not found  subCategory by {subCategoryId}", subCategory?.Id);
            return new Result(false, "SubCategory Not Found", null, 404);
        }

        if (subCategory.StoreId != storeId)
        {
            logger.LogError("subCategory {subCategoryId} is not belong to {storeId}", subCategory?.Id, storeId);
            return new Result(false, "the SubCategory does not belong to this store", null, 403);
        }

        await unitOfWork.SubCategoryRepository.Delete(id);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("error while deleting subcategory");
            return new Result(false, "error while deleting subcategory", null, 403);
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.StoreSubCategoriesKey);

        logger.LogInformation("end delete subCategory");

        return new Result(true, null, null, 204);
    }

    public async Task<Result> GetSubCategories(Guid storeId, int page, int length)
    {
        logger.LogInformation("start getting subCategory page by page by storeId");

        var subCategories = await cache.GetOrCreateAsync(
            MemoryCacheKeys.StoreSubCategoriesKey + '/' + storeId + '/' + page,
            async ct =>
            {
                var subCategories = (await unitOfWork.SubCategoryRepository
                        .GetSubCategories(storeId, page, length))
                    .ToList();
                return subCategories;
            },
            tags: [MemoryCacheKeys.StoreSubCategoriesKey]);

        logger.LogInformation("end getting subCategory page by page by storeId");
        return new Result(true, null, subCategories, 200);
    }

    public async Task<Result> GetSubCategoryAll(
        Guid adminId,
        int page,
        int length)
    {
        logger.LogInformation("start getting subCategory page by page by adminId");

        var user = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = user.IsValidateFunc(isAdmin: true);

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", user?.Id, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var subcategories = await cache.GetOrCreateAsync(
            MemoryCacheKeys.StoreSubCategoriesKey + "/admin/" + adminId + '/' + page,
            async ct =>
            {
                var subcategories = (await unitOfWork.SubCategoryRepository
                        .GetSubCategories(page, length))
                    .ToList();
                return subcategories;
            },
            tags: [MemoryCacheKeys.StoreSubCategoriesKey]);

        logger.LogInformation("end getting subCategory page by page by adminId");
        return new Result(true, null, subcategories, 200);
    }
}