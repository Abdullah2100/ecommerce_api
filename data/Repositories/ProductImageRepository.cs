using api.application;
using api.domain.entity;
using data.Interface;
using data.util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace data.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="ProductImage"/> entities.
/// Provides functionality for adding, retrieving, and deleting product images.
/// </summary>
/// <param name="context">The database context used for data access.</param>
/// <param name="logger">The logger used to log generated SQL queries.</param>
public class ProductImageRepository(
    AppDbContext context,
    ILogger<ProductImageRepository> logger
) : IProductImageRepository
{
    /// <summary>
    /// Deletes a product image based on the product identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <remarks>
    /// Note: The current implementation throws an <see cref="ArgumentNullException"/> if a match is found.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown if an image for the product exists.</exception>
    public void DeleteProductImages(Guid id)
    {
        var query = context
        .ProductImages
        .AsNoTracking()
        .Where(p => p.ProductId == id);

        ClsUtil.logSql<ProductImageRepository>(
            logger,
            query.ToQueryString()
        );

        var result = query.FirstOrDefault();
        if (result != null) throw new ArgumentNullException();
        if (result != null) context.ProductImages.Remove(result);
    }

    /// <summary>
    /// Deletes a specific collection of images for a product by their paths and product identifier.
    /// </summary>
    /// <param name="images">The collection of image paths to delete.</param>
    /// <param name="id">The unique identifier of the product.</param>
    public void DeleteProductImages(ICollection<string> images, Guid id)
    {
        foreach (var t in images)
        {
            var imagePath = ClsUtil.RemoveAdditionalPath(t);

            var query = context
            .ProductImages
            .AsNoTracking()
            .Where(pi => pi.Path == imagePath && pi.ProductId == id);

            ClsUtil.logSql<ProductImageRepository>(
                logger,
                query.ToQueryString()
            );

            var result = query.FirstOrDefault();
            if (result is not null)
                context.ProductImages.Remove(result);
        }
    }

    /// <summary>
    /// Adds a collection of <see cref="ProductImage"/> entities to the database context.
    /// </summary>
    /// <param name="productImage">The collection of product images to add.</param>
    public void AddProductImage(ICollection<ProductImage> productImage)
    {
        for (var i = 0; i < productImage.Count; i++)
        {
            context.ProductImages.Add(productImage.ElementAt(i));
        }
    }

    /// <summary>
    /// Retrieves all image paths associated with a specific product ID without tracking.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of image path strings.</returns>
    public async Task<ICollection<string>> GetProductImages(Guid id)
    {
        var query = context.ProductImages
            .AsNoTracking()
            .Where(pi => pi.ProductId == id)
            .Select(pi => pi.Path);

        ClsUtil.logSql<ProductImageRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.ToListAsync();
    }

    /// <summary>
    /// Adds a new <see cref="ProductImage"/> entity to the database context.
    /// </summary>
    /// <param name="entity">The product image entity to add.</param>
    public void Add(ProductImage entity)
    {
        context.Add(entity);
    }

    /// <summary>
    /// Updates an existing <see cref="ProductImage"/> entity in the database context.
    /// </summary>
    /// <param name="entity">The product image entity with updated values.</param>
    public void Update(ProductImage entity)
    {
        context.Update(entity);
    }
}