using api.application;
using api.domain.entity;
using data.dto.Request;
using data.Interface;
using Microsoft.EntityFrameworkCore;

namespace data.Repositories;

/// <summary>
/// Repository class for managing database operations related to product variants.
/// Implements the <see cref="IProductVariantRepository"/> interface using Entity Framework Core.
/// </summary>
/// <param name="context">The application database context dependency injected via primary constructor.</param>
public class ProductVariantRepository(AppDbContext context) : IProductVariantRepository
{
    /// <summary>
    /// Retrieves a specific product variant based on its product ID and variant ID.
    /// </summary>
    /// <param name="productId">The unique identifier of the parent product.</param>
    /// <param name="id">The unique identifier of the specific product variant.</param>
    /// <returns>The matching <see cref="ProductVariant"/> if found; otherwise, <c>null</c>.</returns>
    public async Task<ProductVariant?> GetProductVariant(Guid productId, Guid id)
    {
        return await context.ProductVariants
            .FirstOrDefaultAsync(or => or.ProductId == productId && or.Id == id);
    }

    /// <summary>
    /// Processes a collection of product variants to either update existing ones or add new ones.
    /// </summary>
    /// <param name="productVariants">The collection of product variants to be saved.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
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
    /// Deletes all product variants associated with a specific product ID.
    /// </summary>
    /// <param name="productId">The unique identifier of the product whose variants should be removed.</param>
    public async Task DeleteProductVariantByProductId(Guid productId)
    {
        var result = await context.ProductVariants.Where(p => p.ProductId == productId).ToListAsync();
        if (result.Count == 0) return;
        context.ProductVariants.RemoveRange(result);
    }

    /// <summary>
    /// Deletes specific product variants that match the provided criteria from a DTO collection and product ID.
    /// </summary>
    /// <param name="productVariants">The collection of DTOs containing variant identifiers and names to match against.</param>
    /// <param name="productId">The unique identifier of the parent product.</param>
    public async Task DeleteProductVariant(ICollection<CreateProductVariantDto> productVariants, Guid productId)
    {
        try
        {
            for (var i = 0; i < productVariants.Count; i++)
            {
                var result = await context.ProductVariants
                    .FirstOrDefaultAsync(pv =>
                        pv.ProductId == productId && pv.VariantId == productVariants.ElementAt(i).VariantId &&
                        pv.Name == productVariants.ElementAt(i).Name
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
    /// Tracks a new product variant entity for insertion into the database context.
    /// </summary>
    /// The product variant entity to add.</param>
    public void Add(ProductVariant entity)
    {
        context.ProductVariants.Add(entity);
    }

    /// <summary>
    /// Tracks an existing product variant entity for modification within the database context.
    /// </summary>
    /// The product variant entity to update.</param>
    public void Update(ProductVariant entity)
    {
        context.ProductVariants.Update(entity);
    }
}