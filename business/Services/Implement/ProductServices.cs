using api.application;
using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using business.mapper;
using api.util;
using business.Services.Interface;
using data.util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;

namespace business.Services.Implement;

public class ProductServices(
    IConfiguration config,
    IUnitOfWork unitOfWork,
    IFileServices fileServices,
    HybridCache cache
) : IProductServices
{
    private void DeleteProductImages(string rootPath,List<string>? images = null, string? savedThumbnail = null)
    {
        if (images is not null)
            fileServices.DeleteFile(images,rootPath);
        if (savedThumbnail is not null)
            fileServices.DeleteFile(savedThumbnail,rootPath);
    }

    public async Task<Result> GetProductsByStoreId(Guid storeId, int pageNum, int pageSize)
    {
        var productsDto = await cache.GetOrCreateAsync(
            MemoryCacheKeys.ProductsKey + "/store" + storeId + '/' + pageNum,
            async ct =>
            {
                var products = (await unitOfWork.ProductRepository.GetProducts(storeId, pageNum, pageSize))
                    .Select(de => de.ToDto(config["url_file"] ?? ""))
                    .ToList();
                return products;
            },
            tags: [MemoryCacheKeys.ProductsKey]);

        return new Result(true, null, productsDto, 200);
    }

    public async Task<Result> GetProductsByCategoryId(Guid categryId, int pageNum, int pageSize)
    {
        var productsDto = await cache.GetOrCreateAsync(
            MemoryCacheKeys.ProductsKey + "/category" + categryId + '/' + pageNum,
            async ct =>
            {
                var products = (await unitOfWork.ProductRepository.GetProductsByCategory(categryId, pageNum, pageSize))
                    .Select(de => de.ToDto(config["url_file"] ?? ""))
                    .ToList();
                return products;
            },
            tags: [MemoryCacheKeys.ProductsKey]);

        return new Result(true, null, productsDto, 200);
    }

    public async Task<Result> GetProducts(Guid storeId, Guid subCategoryId, int pageNum, int pageSize)
    {
        var productsDto = await cache.GetOrCreateAsync(
            MemoryCacheKeys.ProductsKey + '/' + storeId + '/' + subCategoryId + '/' + pageNum,
            async ct =>
            {
                var products = (await unitOfWork.ProductRepository.GetProducts(storeId, subCategoryId, pageNum, pageSize))
                    .Select(de => de.ToDto(config["url_file"] ?? ""))
                    .ToList();
                return products;
            },
            tags: [MemoryCacheKeys.ProductsKey]);

        return new Result(true, null, productsDto, 200);
    }

    public async Task<Result> GetProducts(int pageNum, int pageSize)
    {
        var productsDto = await cache.GetOrCreateAsync(
            MemoryCacheKeys.ProductsKey + '/' + pageNum,
            async ct =>
            {
                var products = (await unitOfWork.ProductRepository.GetProducts(pageNum, pageSize))
                    .Select(de => de.ToDto(config["url_file"] ?? ""))
                    .ToList();
                return products;
            },
            tags: [MemoryCacheKeys.ProductsKey]);

        return new Result(true, null, productsDto, 200);
    }

    public async Task<Result> GetProductsForAdmin(Guid adminId, int pageNum, int pageSize)
    {
        var user = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = user.IsValidateFunc(false);

        if (validationResult is not null)
        {
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var productsDto = await cache.GetOrCreateAsync(
            MemoryCacheKeys.ProductsKey + '/' + adminId + '/' + pageNum,
            async ct =>
            {
                var products = (await unitOfWork.ProductRepository.GetProducts(pageNum, pageSize))
                    .Select(de => de.ToAdminDto(config["url_file"] ?? ""))
                    .ToList();
                return products;
            },
            tags: [MemoryCacheKeys.ProductsKey]);

        return new Result(true, null, productsDto, 200);
    }

    public async Task<Result> GetProductsPagesForAdmin(Guid adminId, int length = 25)
    {
        var admin = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = admin.IsValidateFunc(false);

        if (validationResult is not null)
        {
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var productPageLength = await unitOfWork.ProductRepository.GetProductPages();
        var productsPerPage = productPageLength != null ? (int)Math.Ceiling((double)productPageLength / length) : 0;

        return new Result(true, null, productsPerPage, 200);
    }

    public async Task<Result> CreateProducts(Guid userId, CreateProductDto productDto,string rootPath)
    {
        var user = await unitOfWork.UserRepository.GetUser(userId);
        var validationResult = user.IsValidateFunc(false, true);

        if (validationResult is not null)
        {
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var isExistCurrency = await unitOfWork.CurrencyRepository.isExist(productDto.Symbol);
        if (!isExistCurrency)
        {
            return new Result(false, "Currency is Not Exist", null, 404);
        }

        var savedThumbnail = await fileServices.SaveFile(productDto.Thumbnail, EnImageType.Product,rootPath);
        var savedImage = await fileServices.SaveFile(productDto.Images, EnImageType.Product,rootPath);

        if (savedImage is null || savedThumbnail is null)
        {
            DeleteProductImages(rootPath,savedImage, savedThumbnail);
            return new Result(false, "error while saving image ", null, 500);
        }

        var id = ClsUtil.GenerateGuid();
        var images = savedImage.Select(pi => new ProductImage { Id = ClsUtil.GenerateGuid(), Path = pi, ProductId = id }).ToList();

        if (images.Count > 20)
        {
            DeleteProductImages(rootPath,savedImage, savedThumbnail);
            return new Result(false, "product image can maximum has 20 images", null, 403);
        }

        List<ProductVariant>? productVariants = null;
        if (productDto.ProductVariants is not null)
        {
            productVariants = productDto.ProductVariants.Select(pv => new ProductVariant
            {
                Id = ClsUtil.GenerateGuid(),
                Name = pv.Name,
                Percentage = pv.Percentage,
                ProductId = id,
                VariantId = pv.VariantId,
                OrderProductsVariants = null
            }).ToList();
        }

        if (productVariants is not null && productVariants.Count > 20)
        {
            DeleteProductImages(rootPath,savedImage, savedThumbnail);
            return new Result(false, "productVariant  can maximum has 20 images", null, 403);
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
            await unitOfWork.ProductVariantRepository.SaveProductVariants(productVariants);

        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            DeleteProductImages(rootPath,savedImage, savedThumbnail);
            return new Result(false, "error while adding product", null, 500);
        }

        product = await unitOfWork.ProductRepository.GetProduct(product.Id);
        var productToDto = product?.ToDto(config["url_file"] ?? "");

        await cache.RemoveByTagAsync(MemoryCacheKeys.ProductsKey);
        return new Result(true, null, productToDto, 201);
    }

    public async Task<Result> UpdateProducts(Guid userId, UpdateProductDto productDto,string rootPath)
    {
        if (productDto.IsEmpty())
            return new Result(false, "No Product Change Found", null, 400);

        var user = await unitOfWork.UserRepository.GetUser(userId);
        var validationResult = user.IsValidateFunc(false, true);

        if (validationResult is not null)
        {
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        if (productDto.Symbol is not null)
        {
            var isExistCurrency = await unitOfWork.CurrencyRepository.isExist(productDto.Symbol);
            if (!isExistCurrency)
            {
                return new Result(false, "Currency is Not Exist", null, 404);
            }
        }

        if (productDto.SubcategoryId is not null && !(await unitOfWork.SubCategoryRepository.IsExist((Guid)productDto.SubcategoryId!)))
        {
            return new Result(false, "subCategory  is not found ", null, 404);
        }

        var product = await unitOfWork.ProductRepository.GetProduct(productDto.Id, productDto.StoreId);
        if (product is null)
        {
            return new Result(false, "product is not found ", null, 404);
        }

        int result = 0;

        if (productDto.Deletedimages is not null)
        {
            unitOfWork.ProductImageRepository.DeleteProductImages(productDto.Deletedimages, productDto.Id);
            fileServices.DeleteFile(productDto.Deletedimages,rootPath);
        }

        if (productDto.DeletedProductVariants is not null)
        {
            await Task.Run(() => unitOfWork.ProductVariantRepository.DeleteProductVariant(productDto.DeletedProductVariants, productDto.Id));
        }

        string? savedThumbnail = null;
        List<ProductImage>? savedImage = null;

        if (productDto.Thumbnail is not null)
            savedThumbnail = await fileServices.SaveFile(productDto.Thumbnail, EnImageType.Product,rootPath);

        if (productDto.Images is not null)
            savedImage = (await fileServices.SaveFile(productDto.Images, EnImageType.Product,rootPath))
                ?.Select(im => new ProductImage { Id = ClsUtil.GenerateGuid(), Path = im, ProductId = product.Id }).ToList();

        if (savedImage is not null && (savedImage.Count + product?.ProductImages?.Count) > 20)
        {
            DeleteProductImages(rootPath,savedImage.Select(value => value.Path).ToList(), savedThumbnail);
            return new Result(false, "product image can maximum has 20 images", null, 403);
        }

        if ((savedImage?.Count + product?.ProductImages?.Count) < 1)
        {
            DeleteProductImages(rootPath,savedImage?.Select(value => value.Path).ToList(), savedThumbnail);
            return new Result(false, "product image must  has 2 image at least ", null, 403);
        }

        List<ProductVariant>? productVariants = null;
        if (productDto.ProductVariants is not null)
        {
            productVariants = productDto.ProductVariants.Select(pv => new ProductVariant
            {
                Id = ClsUtil.GenerateGuid(),
                Name = pv.Name,
                Percentage = pv.Percentage,
                ProductId = product!.Id,
                VariantId = pv.VariantId,
                OrderProductsVariants = null
            }).ToList();
        }

        if (productVariants is not null && (productVariants.Count + product?.ProductVariants?.Count) > 20)
        {
            DeleteProductImages(rootPath,savedImage?.Select(value => value.Path).ToList(), savedThumbnail);
            return new Result(false, "product variant can maximum has 20 variants", null, 403);
        }

        if (productVariants is not null)
            await unitOfWork.ProductVariantRepository.SaveProductVariants(productVariants);

        if (savedImage is not null)
            await Task.Run(() => unitOfWork.ProductImageRepository.AddProductImage(savedImage));

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
            DeleteProductImages(rootPath,savedImage?.Select(value => value.Path).ToList(), savedThumbnail);
            return new Result(false, "error while updating product", null, 500);
        }

        product = await unitOfWork.ProductRepository.GetProduct(product.Id);
        var productToDto = product?.ToDto(config["url_file"] ?? "");

        await cache.RemoveByTagAsync(MemoryCacheKeys.ProductsKey);
        return new Result(true, null, productToDto, 200);
    }

    public async Task<Result> DeleteProducts(Guid userId, Guid storeId, Guid id,string rootPath)
    {
        var user = await unitOfWork.UserRepository.GetUser(userId);
        var validationResult = user.IsValidateFunc(false, true);

        if (validationResult is not null)
        {
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var product = await unitOfWork.ProductRepository.GetProduct(id, user?.Store?.Id ?? Guid.CreateVersion7());
        if (product is null || id != product.Id || product.Store.Id != storeId)
        {
            return new Result(false, "product is not found ", null, 404);
        }

        unitOfWork.ProductRepository.Delete(product.Id);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new Result(false, "product had link with some order", null, 409);
        }

        if (product.ProductImages is not null)
            foreach (var image in product.ProductImages)
                fileServices.DeleteFile(image.Path,rootPath);

        if (product?.Thumbnail is not null)
            fileServices.DeleteFile(product.Thumbnail,rootPath);

        await cache.RemoveByTagAsync(MemoryCacheKeys.ProductsKey);
        return new Result(true, null, null, 204);
    }
}