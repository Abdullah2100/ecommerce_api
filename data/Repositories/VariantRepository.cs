using api.application;
using api.domain.entity;
using data.Interface;
using Microsoft.EntityFrameworkCore;

namespace data.Repositories;
/// <summary>
/// Provides data access operations for <see cref="Variant"/> entities.
/// Supports retrieving variants with pagination, checking for existing variants,
/// and performing basic create, update, and delete operations.
/// </summary>
/// <param name="context">The database context used to access variant data.</param>
public class VariantRepository(AppDbContext context) : IVariantRepository
{
    /// <summary>
    /// Retrieves a paginated collection of variants.
    /// </summary>
    /// <param name="page">The one-based page number to retrieve.</param>
    /// <param name="length">The maximum number of variants to return.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// contains the requested page of variants.
    /// </returns>
    public async Task<ICollection<Variant>> GetAllAsync(int page, int length)
    {
        return await context
            .Variants
            .AsNoTracking()
            .Skip((page - 1) * length)
            .Take(length)
            .ToListAsync();
    }

    /// <summary>
    /// Adds a variant to the database context.
    /// The changes are not persisted until the context is saved.
    /// </summary>
    /// <param name="entity">The variant entity to add.</param>
    public void Add(Variant entity)
    {
        context.Variants.AddAsync(entity);
    }

    /// <summary>
    /// Marks an existing variant as modified in the database context.
    /// The changes are not persisted until the context is saved.
    /// </summary>
    /// <param name="entity">The variant entity containing the updated values.</param>
    public void Update(Variant entity)
    {
        context.Variants.Update(entity);
    }

    /// <summary>
    /// Removes the variant with the specified identifier from the database context.
    /// The changes are not persisted until the context is saved.
    /// </summary>
    /// <param name="id">The unique identifier of the variant to delete.</param>
    public async Task Delete(Guid id)
    {
        var variants = await context
            .Variants
            .Where(i => i.Id == id)
            .ToListAsync();
        if(variants.Count==0)return;

        context.Variants.RemoveRange(variants);
    }

    /// <summary>
    /// Retrieves a variant by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the variant.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// contains the variant if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<Variant?> GetVariant(Guid id)
    {
        return await context
            .Variants
            .FindAsync(id);
    }

    /// <summary>
    /// Retrieves a paginated collection of variants.
    /// </summary>
    /// <param name="page">The one-based page number to retrieve.</param>
    /// <param name="length">The maximum number of variants to return.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// contains the requested page of variants.
    /// </returns>
    public async Task<ICollection<Variant>> GetVariants(int page, int length)
    {
        var variants = await context
            .Variants
            .AsNoTracking()
            .Skip((page - 1) * length)
            .Take(length)
            .ToListAsync();

        return variants;
    }

    /// <summary>
    /// Gets the total number of pages required to display the variants
    /// using the specified number of variants per page.
    /// </summary>
    /// <param name="variantPerPage">
    /// The maximum number of variants displayed on each page.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// contains the calculated number of pages.
    /// </returns>
    public async Task<int> GetVariantCount(int variantPerPage)
    {
        int count = await context
            .Stores
            .AsNoTracking()
            .CountAsync();

        if (count == 0)
            return 0;

        count = (int)Math.Ceiling((double)count / variantPerPage);

        return count;
    }

    /// <summary>
    /// Determines whether a variant with the specified identifier exists.
    /// </summary>
    /// <param name="id">The unique identifier of the variant.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// is <c>true</c> if the variant exists; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> IsExist(Guid id)
    {
        return await context
            .Variants
            .AsNoTracking()
            .AnyAsync(i => i.Id == id);
    }

    /// <summary>
    /// Determines whether a variant with the specified name exists.
    /// </summary>
    /// <param name="name">The name of the variant to check.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// is <c>true</c> if a variant with the specified name exists;
    /// otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> IsExist(string name)
    {
        return await context
            .Variants
            .AsNoTracking()
            .AnyAsync(i => i.Name == name);
    }

    /// <summary>
    /// Determines whether another variant with the specified name exists,
    /// excluding the variant with the specified identifier.
    /// This is useful for validating name uniqueness during updates.
    /// </summary>
    /// <param name="name">The variant name to check.</param>
    /// <param name="id">
    /// The unique identifier of the variant to exclude from the check.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// is <c>true</c> if another variant with the specified name exists;
    /// otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> IsExist(string name, Guid id)
    {
        return await context
            .Variants
            .AsNoTracking()
            .AnyAsync(i => i.Name == name && i.Id != id);
    }
}