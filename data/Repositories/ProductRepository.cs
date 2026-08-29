using api.application;
using api.domain.entity;
using data.Interface;
using data.dto.Request;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="ProductVariant"/> entities.
/// Handles the mapping of variants (like size or color) to specific products.
/// </summary>
/// <param name="context">The database context used for data access.</param>
public class ProductVariantRepository(AppDbContext context) : IProductVariantRepository
{
    /// <summary>
    /// Retrieves a specific product variant by its unique identifier and associated product identifier.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <param name="id">The unique identifier of the product variant.</param>
    /// <returns>A task representing the asynchronous operation, returning the product variant if found; otherwise, <c>null</c>.</returns>
    public async Task<ProductVariant?> GetProductVariant(Guid productId, Guid id)
    {
        return await context.ProductVariants
            .FirstOrDefaultAsync(or => or.ProductId == productId && or.Id == id);
    }

    /// <summary>
    /// Saves a collection of product variants by either adding new ones or updating existing ones.
    /// Variants with a non-null <c>Id</c> are updated, while those with a null <c>Id</c> are added.
    /// </summary>
    /// <param name="productVariants">The collection of product variants to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SaveProductVariants(ICollection<ProductVariant> productVariants)
    {
        for (var i = 0; i < productVariants.Count; i++)
        {
            if (productVariants.ElementAt(i)?.Id is not null)
                await Task.Run(() => Update(productVariants.ElementAt(i)));
            else
                await Task.Run(() => Add(productVariants.ElementAt(i)));
        }
    }

    /// <summary>
    /// Deletes all variants associated with a specific product ID.
    /// </summary>
    /// <param name="productId">The unique identifier of the product whose variants should be removed.</param>
    public void DeleteProductVariantByProductId(Guid productId)
    {
        var result = context.ProductVariants.Where(p => p.ProductId == productId).ToICollection();
        context.ProductVariants.RemoveRange(result);
    }

    /// <summary>
    /// Deletes a specific set of variants for a product based on a collection of DTOs.
    /// Matches variants by <c>ProductId</c>, <c>VariantId</c>, and <c>Name</c>.
    /// </summary>
    /// <param name="productVariants">The collection of variant data identifying which records to delete.</param>
    /// <param name="productId">The unique identifier of the product.</param>
    public void DeleteProductVariant(ICollection<CreateProductVariantDto> productVariants, Guid productId)
    {
        try
        {
            for (var i = 0; i < productVariants.Count; i++)
            {
                var result = context.ProductVariants
                    .FirstOrDefault(pv =>
                        pv.ProductId == productId && pv.VariantId == productVariants[i].VariantId &&
                        pv.Name == productVariants[i].Name
                    );
                if (result is not null)
                    context.ProductVariants.Remove(result);
            }
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
    }

    /// <summary>
    /// Tracks a new product variant entity to be added to the database.
    /// </summary>
    /// <param name="entity">The product variant entity to add.</param>
    public void Add(ProductVariant entity)
    {
        context.ProductVariants.Add(entity);
    }

    /// <summary>
    /// Updates an existing product variant entity in the database context.
    /// </summary>
    /// <param name="entity">The product variant entity with updated values.</param>
    public void Update(ProductVariant entity)
    {
        context.ProductVariants.Update(entity);
    }
}