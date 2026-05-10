using api.application.Interface;
using api.application.Result;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services;

public class CategoryServices(
    IWebHostEnvironment host,
    IConfig config,
    IUnitOfWork unitOfWork,
    IFileServices fileService)
    : ICategoryServices
{
    public async Task<IActionResult> CreateCategory(CreateCategoryDto categoryDto, Guid adminId)
    {
        User? user = await unitOfWork.UserRepository
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

        string? imagePath = await fileService
            .SaveFile(categoryDto.Image,
                EnImageType.Category);

        if (imagePath is null)

            return new ObjectResult("there error while saving image to server")
                { StatusCode = StatusCodes.Status500InternalServerError };

        Guid categoryId = ClsUtil.GenerateGuid();

        Category category = new Category
        {
            Id = categoryId,
            Name = categoryDto.Name,
            Image = imagePath,
            IsBlocked = false,
            OwnerId = user!.Id
        };
        unitOfWork.CategoryRepository.Add(category);
        int result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            fileService.DeleteFile(imagePath);

            return new ObjectResult("error while adding category")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }


        var categoryToDto = category?.ToDto(config.GetKey("url_file"));
        return new ObjectResult(categoryToDto)
            { StatusCode = StatusCodes.Status201Created };
    }

    public async Task<IActionResult> UpdateCategory(UpdateCategoryDto categoryDto, Guid adminId)
    {
        if (categoryDto.IsEmpty())
            return new ObjectResult("no change found")
                { StatusCode = StatusCodes.Status200OK };


        User? user = await unitOfWork.UserRepository
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


        //this for production to prevent category create overload on vps to keep the size of vps fit 
        int categoryCount = await unitOfWork.CategoryRepository.GetCategoriesCount();

        if (categoryCount > 20)
        {
            var bannersRandom = await unitOfWork.CategoryRepository.GetCategories(20);
            var imagesList = bannersRandom.Select(b => b.Image).ToList();
            fileService.DeleteFile(imagesList);
            unitOfWork.CategoryRepository.Delete(bannersRandom);
        }
        //end

        Category? category = await unitOfWork.CategoryRepository.GetCategory(categoryDto.Id);


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
        int result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            if (image != null)
                fileService.DeleteFile(image);

            return new ObjectResult("error while update category")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }


        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> DeleteCategory(Guid categoryId, Guid adminId)
    {
        User? user = await unitOfWork.UserRepository
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
        
        int result = await unitOfWork.SaveChanges();

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
        List<CategoryDto> categories = (await unitOfWork.CategoryRepository.GetCategories(pageNumber, pageSize))
            .Select(ca => ca.ToDto(config.GetKey("url_file")))
            .ToList();
        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };

    }
}