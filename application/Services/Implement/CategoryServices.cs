using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Sats.PostgresDistributedCache;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace api.application.Services.Implement;

public class CategoryServices(
    IConfiguration config,
    IUnitOfWork unitOfWork,
    IFileServices fileService,
    HybridCache cache,
    ILogger<CategoryServices> logger)
    : ICategoryServices
{
    public async Task<IActionResult> CreateCategory(CreateCategoryDto categoryDto, Guid adminId)
    {
        logger.LogInformation("start calling create category");

        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc();
        if (validationResult is not null)
        {
            logger.LogInformation(
                "not valid user {userId} trying to crate  {categoryName} with validation error {error} from  create category",
                adminId,
                categoryDto.Name ?? "", validationResult.Item1 ?? "");
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        if (await unitOfWork.CategoryRepository.IsExist(categoryDto.Name))
        {
            logger.LogInformation("category is exist {categoryName} from  create category", categoryDto.Name);

            return new ObjectResult("there are category with the same name")
                { StatusCode = StatusCodes.Status409Conflict };
        }

        var imagePath = await fileService
            .SaveFile(categoryDto.Image,
                EnImageType.Category);

        if (imagePath is null)
        {
            logger.LogInformation("could not save category image to local api from  create category");

            return new ObjectResult("there error while saving image to server")
                { StatusCode = StatusCodes.Status500InternalServerError };
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
            logger.LogInformation("could not saved category {categoryName} to db for sto from  create category",
                categoryDto.Name);

            fileService.DeleteFile(imagePath);

            return new ObjectResult("error while adding category")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.CategoriesKey);

        var categoryToDto = category?.ToDto(config["url_file"] ?? "");

        logger.LogInformation("end calling create category");

        return new ObjectResult(categoryToDto)
            { StatusCode = StatusCodes.Status201Created };
    }

    public async Task<IActionResult> UpdateCategory(UpdateCategoryDto categoryDto, Guid adminId)
    {
        logger.LogInformation("start calling update category");

        if (categoryDto.IsEmpty())
            return new ObjectResult("no change found")
                { StatusCode = StatusCodes.Status200OK };


        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc();
        if (validationResult is not null)
        {
            logger.LogInformation(
                "not valid user {userId} trying to update category {categoryName} with validation error {error} from  update category",
                adminId,
                categoryDto.Name ?? "", validationResult.Item1 ?? "");

            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        if (categoryDto.Name is not null)
            if (await unitOfWork.CategoryRepository.IsExist(categoryDto.Name, categoryDto.Id))
            {
                logger.LogInformation("category {categoryName} is already exists  from  update category",
                    categoryDto.Name);
                return new ObjectResult("there are category with the same name")
                    { StatusCode = StatusCodes.Status409Conflict };
            }


        var category = await unitOfWork.CategoryRepository.GetCategory(categoryDto.Id);


        if (category is null)
        {
            logger.LogInformation("not exist category  from  update category");
            return new ObjectResult("category not found") { StatusCode = StatusCodes.Status404NotFound };
        }

        string? image = null;

        if (categoryDto?.Image is not null)
        {
            if (categoryDto?.Image is not null)
                fileService.DeleteFile(category.Image);
            image = await fileService
                .SaveFile(categoryDto!.Image!,
                    EnImageType.Category);

            fileService.DeleteFile(category.Image);
        }

        category.Name = categoryDto?.Name ?? category.Name;
        category.Image = image ?? category.Image;
        category.UpdatedAt = DateTime.Now;

        unitOfWork.CategoryRepository.Update(category);
        var result = await unitOfWork.SaveChanges();

        if (result != 0)
        {
            logger.LogInformation("could not update the category from ef in   update category");

            return new ObjectResult(null)
                { StatusCode = StatusCodes.Status204NoContent };
        }

        if (image != null)
            fileService.DeleteFile(image);

        await cache.RemoveByTagAsync(MemoryCacheKeys.CategoriesKey);

        logger.LogInformation("end calling update category");

        return new ObjectResult("error while update category")
            { StatusCode = StatusCodes.Status500InternalServerError };
    }

    public async Task<IActionResult> DeleteCategory(Guid categoryId, Guid adminId)
    {
        logger.LogInformation("start calling delete category");

        var user = await unitOfWork.UserRepository
            .GetUser(adminId);
        var validationResult = user.IsValidateFunc(true);
        if (validationResult is not null)
        {
            logger.LogInformation(
                "not valid user {userId} trying to delete category {categoryId} with validation error {error} from  delete category",
                adminId,
                categoryId, validationResult.Item1 ?? "");
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var category = await unitOfWork.CategoryRepository.GetCategory(categoryId);
        if (category is null)
        {
            logger.LogInformation("category {categoryId} is not exist at   delete category", categoryId);
            return new ObjectResult("category not found") { StatusCode = StatusCodes.Status404NotFound };
        }

        fileService.DeleteFile(category.Image);

        unitOfWork.CategoryRepository.Delete(categoryId);

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogInformation("could not delete category in ef from delete category");

            return new ObjectResult("error while delete category")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.CategoriesKey);
        logger.LogInformation("end calling delete category");

        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> GetCategories(int pageNumber, int pageSize)
    {
        logger.LogInformation("start calling getting  categories");

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
        logger.LogInformation("end calling getting  categories");

        return new ObjectResult(categories)
            { StatusCode = StatusCodes.Status200OK };
    }
}