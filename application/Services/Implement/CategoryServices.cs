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

public class CategoryServices(
    IConfiguration config,
    IUnitOfWork unitOfWork,
    IFileServices fileService)
    : ICategoryServices
{
    public async Task<IActionResult> CreateCategory(CreateCategoryDto categoryDto, Guid adminId)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc();
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        if (await unitOfWork.CategoryRepository.IsExist(categoryDto.Name))
        {
            return new ObjectResult("there are category with the same name")
                { StatusCode = StatusCodes.Status409Conflict };
        }

        var imagePath = await fileService
            .SaveFile(categoryDto.Image,
                EnImageType.Category);

        if (imagePath is null)

            return new ObjectResult("there error while saving image to server")
                { StatusCode = StatusCodes.Status500InternalServerError };

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
            fileService.DeleteFile(imagePath);

            return new ObjectResult("error while adding category")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }


        var categoryToDto = category?.ToDto(config["url_file"]??"");
        return new ObjectResult(categoryToDto)
            { StatusCode = StatusCodes.Status201Created };
    }

    public async Task<IActionResult> UpdateCategory(UpdateCategoryDto categoryDto, Guid adminId)
    {
        if (categoryDto.IsEmpty())
            return new ObjectResult("no change found")
                { StatusCode = StatusCodes.Status200OK };


        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc();
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        if (categoryDto.Name is not null)
            if (await unitOfWork.CategoryRepository.IsExist(categoryDto.Name, categoryDto.Id))
            {
                return new ObjectResult("there are category with the same name")
                    { StatusCode = StatusCodes.Status409Conflict };
            }


        

        var category = await unitOfWork.CategoryRepository.GetCategory(categoryDto.Id);


        if (category is null)
        {
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
            return new ObjectResult(null)
                { StatusCode = StatusCodes.Status204NoContent };

        if (image != null)
            fileService.DeleteFile(image);

    
        return new ObjectResult("error while update category")
            { StatusCode = StatusCodes.Status500InternalServerError };
    }

    public async Task<IActionResult> DeleteCategory(Guid categoryId, Guid adminId)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(adminId);
        var validationResult = user.IsValidateFunc(true);
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var category = await unitOfWork.CategoryRepository.GetCategory(categoryId);
        if (category is null)
        {
            return new ObjectResult("category not found") { StatusCode = StatusCodes.Status404NotFound };
        }

        fileService.DeleteFile(category.Image);

        unitOfWork.CategoryRepository.Delete(categoryId);

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("error while delete category")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> GetCategories(int pageNumber, int pageSize)
    {
        var categories = (await unitOfWork.CategoryRepository.GetCategories(pageNumber, pageSize))
            .Select(ca => ca.ToDto(config["url_file"]??""))
            .ToList();
        return new ObjectResult(categories)
            { StatusCode = StatusCodes.Status200OK };
    }
}