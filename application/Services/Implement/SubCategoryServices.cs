using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services.Implement;

public class SubCategoryServices(
    IUnitOfWork unitOfWork)
    : ISubCategoryServices
{
    public async Task<IActionResult> CreateSubCategory(
        Guid storeId,
        CreateSubCategoryDto subCategoryDto
    )
    {
        Store? store = await unitOfWork.StoreRepository
            .GetStore(storeId);


        if (store is not null)
        {
            return new ObjectResult("Store Not Found")
                { StatusCode = StatusCodes.Status404NotFound };
        }


        var count = await unitOfWork.SubCategoryRepository.GetSubCategoriesCount(storeId);

        if (count == 20)
        {
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
            return new ObjectResult("error while adding new subcategory")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var subCategoryToDto = subCategory.ToDto();

        return new ObjectResult(subCategoryToDto)
            { StatusCode = StatusCodes.Status201Created };
    }

    public async Task<IActionResult> UpdateSubCategory(
        Guid storeId,
        UpdateSubCategoryDto subCategoryDto
    )
    {
        if (subCategoryDto.IsEmpty())
            return new ObjectResult("No Change Found At Data")
                { StatusCode = StatusCodes.Status400BadRequest };


        var store = await unitOfWork.StoreRepository
            .GetStore(storeId);


        if (store is not null)
        {
            return new ObjectResult("Store Not Found")
                { StatusCode = StatusCodes.Status404NotFound };
        }


        var subCategory = await unitOfWork.SubCategoryRepository
            .GetSubCategory(subCategoryDto.Id);

        if (subCategory is null || subCategory.StoreId != storeId)
        {
            return new ObjectResult("subcategory not found")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        if (subCategoryDto.CategoryId is not null &&
            !(await unitOfWork.CategoryRepository.IsExist((Guid)subCategoryDto.CategoryId))
           )
        {
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
            return new ObjectResult("error while update subcategory")
                { StatusCode = StatusCodes.Status404NotFound };
        }


        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> DeleteSubCategory(Guid id, Guid storeId)
    {
        var store = await unitOfWork.StoreRepository
            .GetStore(storeId);


        if (store is not null)
        {
            return new ObjectResult("Store Not Found")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        var subCategory = await unitOfWork.SubCategoryRepository
            .GetSubCategory(id);

        if (subCategory is null)
        {
            return new ObjectResult("SubCategory Not Found")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        if (subCategory.StoreId != storeId)
        {
            return new ObjectResult("the SubCategory does not belong to this store")
                { StatusCode = StatusCodes.Status403Forbidden };
        }

        unitOfWork.SubCategoryRepository.Delete(id);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
            return new ObjectResult("error while deleting subcategory")
                { StatusCode = StatusCodes.Status403Forbidden };


        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> GetSubCategories(Guid id, int page, int length)
    {
        var subCategories = (await unitOfWork.SubCategoryRepository
                .GetSubCategories(id, page, length))
            .Select(su => su.ToDto())
            .ToList();
        return (subCategories.Count > 0) switch
        {
            true => new ObjectResult(subCategories)
                { StatusCode = StatusCodes.Status200OK },
            _ =>
                new ObjectResult(new List<SubCategoryDto>())
                    { StatusCode = StatusCodes.Status200OK },
        };
    }

    public async Task<IActionResult> GetSubCategoryAll(
        Guid adminId,
        int page,
        int length)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(adminId);
        var validationResult = user.IsValidateFunc(isAdmin: true);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }


        var subcategories = (await unitOfWork.SubCategoryRepository
                .GetSubCategories(page, length))
            .Select(ba => ba.ToDto())
            .ToList();


        return new ObjectResult(subcategories)
            { StatusCode = StatusCodes.Status200OK };
    }
}