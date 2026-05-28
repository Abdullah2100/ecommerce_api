using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services.Implement;

public class ProductServices(
    IConfiguration config,
    IUnitOfWork unitOfWork,
    IFileServices fileServices
)
    : IProductServices
{
    private void DeleteProductImages(List<string>? images = null, string? savedThumbnail = null)
    {
        if (images is not null)
            fileServices.DeleteFile(images);
        if (savedThumbnail is not null)
            fileServices.DeleteFile(savedThumbnail);
    }

    public async Task<IActionResult> GetProductsByStoreId(
        Guid storeId,
        int pageNum,
        int pageSize
    )
    {
        var productsToDto = (await unitOfWork.ProductRepository
                .GetProducts(storeId, pageNum, pageSize))
            .Select((de) => de.ToDto(config["url_file"]??""))
            .ToList();


        return new ObjectResult(productsToDto)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    public async Task<IActionResult> GetProductsByCategoryId(
        Guid categryId,
        int pageNum,
        int pageSize
    )
    {
        var productsToDto = (await unitOfWork.ProductRepository
                .GetProductsByCategory(categryId, pageNum, pageSize))
            .Select((de) => de.ToDto(config["url_file"]??""))
            .ToList();

        return new ObjectResult(productsToDto)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    public async Task<IActionResult> GetProducts(
        Guid storeId,
        Guid subCategoryId,
        int pageNum,
        int pageSize
    )
    {
        var productsToDto = (await unitOfWork.ProductRepository
                .GetProducts(storeId, subCategoryId, pageNum, pageSize))
            .Select((de) => de.ToDto(config["url_file"]??""))
            .ToList();

        return new ObjectResult(productsToDto)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    public async Task<IActionResult> GetProducts(
        int pageNum,
        int pageSize
    )
    {
        var products = (await unitOfWork.ProductRepository
            .GetProducts(pageNum, pageSize));

        var productsToDto = products.Select((de) => de.ToDto(config["url_file"]??""))
            .ToList();

        return new ObjectResult(productsToDto)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    public async Task<IActionResult> GetProductsForAdmin(Guid adminId,
        int pageNum,
        int pageSize)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc(false);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }


        var productsToDto = (await unitOfWork.ProductRepository
                .GetProducts(pageNum, pageSize))
            .Select((de) => de.ToAdminDto(config["url_file"]??""))
            .ToList();


        return new ObjectResult(productsToDto)
            { StatusCode = StatusCodes.Status200OK };
    }

    public async Task<IActionResult> GetProductsPagesForAdmin(Guid adminId, int length = 25)
    {
        User? admin = await unitOfWork.UserRepository.GetUser(adminId);

        var validationResult = admin.IsValidateFunc(false);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }


        var productPageLength = await unitOfWork.ProductRepository.GetProductPages();

        var productsPerPage = productPageLength != null ? (int)Math.Ceiling((double)productPageLength / length) : 0;

        return new ObjectResult(productsPerPage)
            { StatusCode = StatusCodes.Status200OK };
    }

    public async Task<IActionResult> CreateProducts(
        Guid userId,
        CreateProductDto productDto
    )
    {
        var user = await unitOfWork.UserRepository.GetUser(userId);

        var validationResult = user.IsValidateFunc(false, true);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }


        var isExistCurrency = await unitOfWork.CurrencyRepository.isExist(productDto.Symbol);

        if (!isExistCurrency)
        {
            return new ObjectResult("Currency is Not Exist")
                { StatusCode = StatusCodes.Status404NotFound };
        }


        var savedThumbnail = await fileServices.SaveFile(
            productDto.Thumbnail,
            EnImageType.Product);

        var savedImage = await fileServices.SaveFile(
            productDto.Images,
            EnImageType.Product);

        if (savedImage is null || savedThumbnail is null)
        {
            DeleteProductImages(savedImage, savedThumbnail);

            return new ObjectResult("error while saving image ")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }


        var id = ClsUtil.GenerateGuid();

        var images = savedImage.Select(pi => new ProductImage
            {
                Id = ClsUtil.GenerateGuid(),
                Path = pi,
                ProductId = id
            })
            .ToList();

        if ((images.Count) > 20)
        {
            DeleteProductImages(savedImage, savedThumbnail);

            return new ObjectResult("product image can maximum has 20 images")
                { StatusCode = StatusCodes.Status403Forbidden };
        }


        List<ProductVariant>? productVariants = null;
        if (productDto.ProductVariants is not null)
            productVariants = productDto
                .ProductVariants!.Select(pv =>
                    new ProductVariant
                    {
                        Id = ClsUtil.GenerateGuid(),
                        Name = pv.Name,
                        Percentage = pv.Percentage,
                        ProductId = id,
                        VariantId = pv.VariantId,
                        OrderProductsVariants = null
                    })
                .ToList();

        if (productVariants is not null && productVariants.Count > 20)
        {
            DeleteProductImages(savedImage, savedThumbnail);

            return new ObjectResult("productVariant  can maximum has 20 images")
                { StatusCode = StatusCodes.Status403Forbidden };
        }

        var product = new Product
        {
            Id = id,
            Name = productDto.Name,
            Description = productDto.Description,
            SubcategoryId = productDto.SubcategoryId,
            StoreId = user!.Store!.Id,
            Price = productDto.Price,
            CreatedAt = DateTime.Now,
            UpdatedAt = null,
            Thumbnail = savedThumbnail,
            Symbol = productDto.Symbol,
        };

        unitOfWork.ProductRepository.Add(product);
        unitOfWork.ProductImageRepository.AddProductImage(images);

        if (productVariants is not null)
            await unitOfWork.ProductVariantRepository
                .SaveProductVariants(productVariants);

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            DeleteProductImages(savedImage, savedThumbnail);


            return new ObjectResult("error while adding product")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        product = await unitOfWork.ProductRepository.GetProduct(product.Id);

        var productToDto = product?.ToDto(config["url_file"]??"");

        return new ObjectResult(productToDto)
            { StatusCode = StatusCodes.Status201Created };
    }


    public async Task<IActionResult> UpdateProducts(
        Guid userId, UpdateProductDto productDto
    )
    {
        if (productDto.IsEmpty())
            return new ObjectResult("No Product Change Found")
                { StatusCode = StatusCodes.Status400BadRequest };


        var user = await unitOfWork.UserRepository.GetUser(userId);

        var validationResult = user.IsValidateFunc(false, true);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        if (productDto.Symbol is not null)
        {
            var isExistCurrency = await unitOfWork.CurrencyRepository.isExist(productDto.Symbol);

            if (!isExistCurrency)
            {
                return new ObjectResult("Currency is Not Exist")
                    { StatusCode = StatusCodes.Status404NotFound };
            }
        }

        if (productDto.SubcategoryId is not null &&
            !(await unitOfWork.SubCategoryRepository.IsExist((Guid)productDto!.SubcategoryId!)))
        {
            return new ObjectResult("subCategory  is not found ")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        var product = await unitOfWork.ProductRepository.GetProduct(productDto.Id, productDto.StoreId);

        if (product is null)
        {
            return new ObjectResult("product is not found ")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        int result = 0;

        //delete preview images
        if (productDto.Deletedimages is not null)
        {
            unitOfWork.ProductImageRepository.DeleteProductImages(productDto.Deletedimages,
                productDto.Id);

            fileServices.DeleteFile(productDto.Deletedimages);
        }

        //delete preview product variants
        if (productDto.DeletedProductVariants is not null)
        {
            await Task.Run(() => unitOfWork.ProductVariantRepository.DeleteProductVariant(
                productDto.DeletedProductVariants,
                productDto.Id));
        }

        string? savedThumbnail = null;
        List<ProductImage>? savedImage = null;

        if (productDto.Thumbnail is not null)
            savedThumbnail = await fileServices.SaveFile(
                productDto.Thumbnail,
                EnImageType.Product);

        if (productDto.Images is not null)
            savedImage = (await fileServices.SaveFile(
                    productDto.Images,
                    EnImageType.Product)
                )
                ?.Select(im => new ProductImage
                {
                    Id = ClsUtil.GenerateGuid(),
                    Path = im,
                    ProductId = product.Id
                }).ToList();

        if (savedImage is not null && (savedImage.Count + product?.ProductImages?.Count) > 20)
        {
            DeleteProductImages(savedImage.Select(value => value.Path).ToList(), savedThumbnail);

            return new ObjectResult("product image can maximum has 20 images")
                { StatusCode = StatusCodes.Status403Forbidden };
        }

        if ((savedImage?.Count + product?.ProductImages?.Count) < 1)
        {
            DeleteProductImages(savedImage?.Select(value => value.Path).ToList(), savedThumbnail);

            return new ObjectResult("product image must  has 2 image at least ")
                { StatusCode = StatusCodes.Status403Forbidden };
        }

        List<ProductVariant>? productVariants = null;
        if (productDto.ProductVariants is not null)
            productVariants = productDto
                .ProductVariants!.Select(pv =>
                    new ProductVariant
                    {
                        Id = ClsUtil.GenerateGuid(),
                        Name = pv.Name,
                        Percentage = pv.Percentage,
                        ProductId = product!.Id,
                        VariantId = pv.VariantId,
                        OrderProductsVariants = null
                    })
                .ToList();

        if (productVariants is not null && (productVariants.Count + product?.ProductVariants?.Count) > 20)
        {
            DeleteProductImages(savedImage?.Select(value => value.Path).ToList(), savedThumbnail);


            return new ObjectResult("product variant can maximum has 20 variants")
                { StatusCode = StatusCodes.Status403Forbidden };
        }

        if (productVariants is not null)
        {
            await unitOfWork.ProductVariantRepository.SaveProductVariants(productVariants);
        }

        if (savedImage is not null)
        {
            await Task.Run(() => unitOfWork.ProductImageRepository.AddProductImage(savedImage));
        }

        product!.Name = productDto.Name ?? product.Name;
        product.Description = productDto.Description ?? product.Description;
        product.SubcategoryId = productDto.SubcategoryId ?? product.SubcategoryId;
        product.Price = productDto.Price ?? product.Price;
        product.UpdatedAt = DateTime.Now;
        product.Thumbnail = savedThumbnail ?? product.Thumbnail;
        product.Symbol = productDto.Symbol ?? product.Symbol;
        unitOfWork.ProductRepository.Update(product);

        result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            DeleteProductImages(savedImage?.Select(value => value.Path).ToList(), savedThumbnail);


            return new ObjectResult("error while updating product")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        product = await unitOfWork.ProductRepository.GetProduct(product.Id);

        var productToDto = product?.ToDto(config["url_file"]??"");


        return new ObjectResult(productToDto)
            { StatusCode = StatusCodes.Status200OK };
    }

    public async Task<IActionResult> DeleteProducts(
        Guid userId,
        Guid storeId,
        Guid id
    )
    {
        var user = await unitOfWork.UserRepository.GetUser(userId);

        var validationResult = user.IsValidateFunc(false, true);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var product = await unitOfWork.ProductRepository.GetProduct(id, user?.Store?.Id??Guid.CreateVersion7());

        if (product is null || id != product.Id || product.Store.Id != storeId)
        {
            return new ObjectResult("product is not found ")
                { StatusCode = StatusCodes.Status404NotFound };
        }

        unitOfWork.ProductRepository.Delete(product.Id);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("product had link with some order")
                { StatusCode = StatusCodes.Status409Conflict };
        }

        if (product.ProductImages is not null)
            foreach (var image in product.ProductImages)
            {
                fileServices.DeleteFile(image.Path);
            }

        if (product?.Thumbnail is not null)
            fileServices.DeleteFile(product.Thumbnail);

        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }
}