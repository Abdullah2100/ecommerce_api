using api.application;
using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using business.mapper;
using api.util;
using business.Services.Interface;
using data.dto.Request;
using data.util;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace business.Services.Implement;

public class CategoryServices(
    IConfiguration config,
    IUnitOfWork unitOfWork,
    IFileServices fileService,
    HybridCache cache,
    ILogger<CategoryServices> logger)
    : ICategoryServices
{
    public async Task<Result> CreateCategory(CreateCategoryDto categoryDto, Guid adminId,string contentRoot)
    {
        logger.LogInformation("start calling create category");

        var user = await unitOfWork.UserRepository.GetUser(adminId);
        var validation = user.IsValidateFunc();
        if (validation is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", adminId, validation.Item2);
            return new Result(false, validation.Item1, null, validation.Item2);
        }

        if (await unitOfWork.CategoryRepository.IsExist(categoryDto.Name))
        {
            logger.LogError("category is already exists {categoryName}", categoryDto.Name);
            return new Result(false, "there are category with the same name", null, 409);
        }

        var imagePath = await fileService.SaveFile(categoryDto.Image, EnImageType.Category,contentRoot);

        if (imagePath is null)
        {
            logger.LogError("could not save category image to local");
            return new Result(false, "there error while saving image to server", null, 500);
        }

        var categoryId = ClsUtil.GenerateGuid();

        var category = new Category
        {
            Id = categoryId,
            Name = categoryDto.Name,
            Image = imagePath,
            IsBlocked = false,
            OwnerId = user!.Id
        };
        unitOfWork.CategoryRepository.Add(category);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("could not saved category to db");
            fileService.DeleteFile(imagePath,contentRoot);
            return new Result(false, "error while adding category", null, 500);
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.CategoriesKey);

        var categoryToDto = category?.ToDto(config["url_file"] ?? "");

        logger.LogInformation("end calling create category");
        return new Result(true, null, categoryToDto, 201);
    }

    public async Task<Result> UpdateCategory(UpdateCategoryDto categoryDto, Guid adminId,string contentRoot)
    {
        logger.LogInformation("start calling update category");

        if (categoryDto.IsEmpty())
            return new Result(false, "no change found", null, 200);

        var user = await unitOfWork.UserRepository.GetUser(adminId);
        var validation = user.IsValidateFunc();
        if (validation is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", adminId, validation.Item2);
            return new Result(false, validation.Item1, null, validation.Item2);
        }

        if (categoryDto.Name is not null)
            if (await unitOfWork.CategoryRepository.IsExist(categoryDto.Name, categoryDto.Id))
            {
                logger.LogInformation("category {categoryName} is already exists", categoryDto.Name);
                return new Result(false, "there are category with the same name", null, 409);
            }

        var category = await unitOfWork.CategoryRepository.GetCategory(categoryDto.Id);

        if (category is null)
        {
            logger.LogError("category is not exists in db {categoryId}", category?.Id);
            return new Result(false, "category not found", null, 404);
        }

        string? image = null;

        if (categoryDto?.Image is not null)
        {
            if (categoryDto?.Image is not null)
                fileService.DeleteFile(category.Image,contentRoot);
            image = await fileService.SaveFile(categoryDto!.Image!, EnImageType.Category,contentRoot);
            fileService.DeleteFile(category.Image,contentRoot);
        }

        category.Name = categoryDto?.Name ?? category.Name;
        category.Image = image ?? category.Image;
        category.UpdatedAt = DateTime.Now;

        unitOfWork.CategoryRepository.Update(category);
        var result = await unitOfWork.SaveChanges();

        if (result != 0)
        {
            logger.LogError("could not update the category in db");
            return new Result(true, null, null, 204);
        }

        if (image != null)
            fileService.DeleteFile(image,contentRoot);

        await cache.RemoveByTagAsync(MemoryCacheKeys.CategoriesKey);

        logger.LogInformation("end calling update category");
        return new Result(true, null, null, 204);
    }

    public async Task<Result> DeleteCategory(Guid categoryId, Guid adminId , string contentRoot)
    {
        logger.LogInformation("start calling delete category");

        var user = await unitOfWork.UserRepository.GetUser(adminId);
        var validation = user.IsValidateFunc(true);
        if (validation is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", adminId, validation.Item2);
            return new Result(false, validation.Item1, null, validation.Item2);
        }

        var category = await unitOfWork.CategoryRepository.GetCategory(categoryId);
        if (category is null)
        {
            logger.LogError("category {categoryId} is not exist", categoryId);
            return new Result(false, "category not found", null, 404);
        }

        fileService.DeleteFile(category.Image,contentRoot);
        unitOfWork.CategoryRepository.Delete(categoryId);

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("could not delete category in db");
            return new Result(false, "error while delete category", null, 500);
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.CategoriesKey);
        logger.LogInformation("end calling delete category");
        return new Result(true, null, null, 204);
    }

    public async Task<Result> GetCategories(int pageNumber, int pageSize)
    {
        logger.LogInformation("start calling getting  categories by page");

        var categories = await cache.GetOrCreateAsync(MemoryCacheKeys.CategoriesKey + pageNumber, async ct =>
            {
                var categories = (await unitOfWork
                        .CategoryRepository
                        .GetCategories(pageNumber, pageSize))
                    .Select(ca => ca.ToDto(config["url_file"] ?? ""))
                    .ToList();
                return categories;
            },
            tags: [MemoryCacheKeys.CategoriesKey]);
        logger.LogInformation("end calling getting  categories by page");

        return new Result(true, null, categories, 200);
    }
}