using api.application;
using api.domain.entity;
using data.Interface;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="SubCategory"/> entities.
/// Provides functionality to filter subcategories by store, handle pagination, and manage basic CRUD operations.
/// </summary>
/// <param name="context">The database context used for data access.</param>
public class SubCategoryRepository(AppDbContext context) : ISubCategoryRepository
{
    /// <summary>
    /// Retrieves a specific subcategory by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the subcategory.</param>
    /// <returns>A task representing the asynchronous operation, returning the subcategory if found; otherwise, <c>null</c>.</returns>
    public async Task<SubCategory?> GetSubCategory(Guid id)
    {
        return await context.SubCategories.FindAsync(id);
    }

    /// <summary>
    /// Retrieves a paged collection of subcategories for a specific store, ordered descending.
    /// </summary>
    /// <param name="storeId">The unique identifier of the store.</param>
    /// <param name="pageNumber">The page number to retrieve (1-indexed).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of subcategories.</returns>
    public async Task<ICollection<SubCategory>> GetSubCategories(
        Guid storeId,
        int pageNumber,
        int pageSize
    )
    {
        return await context
            .SubCategories
            .AsNoTracking()
            .Where(su => su.StoreId == storeId)
            .Skip((pageNumber - 1) * pageSize)
            .OrderDescending()
            .Take(pageSize)
            .ToICollectionAsync();
    }

    /// <summary>
    /// Retrieves a paged collection of all subcategories, ordered descending.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve (1-indexed).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of subcategories.</returns>
    public async Task<ICollection<SubCategory>> GetSubCategories(
        int pageNumber,
        int pageSize
    )
    {
        return await context
            .SubCategories
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .OrderDescending()
            .Take(pageSize)
            .ToICollectionAsync();
    }

    /// <summary>
    /// Gets the total number of subcategories associated with a specific store.
    /// </summary>
    /// <param name="storeId">The unique identifier of the store.</param>
    /// <returns>A task representing the asynchronous operation, returning the count of subcategories.</returns>
    public async Task<int> GetSubCategoriesCount(Guid storeId)
    {
        return await context
            .SubCategories
            .AsNoTracking()
            .Where(su => su.StoreId == storeId)
            .CountAsync();
    }

    /// <summary>
    /// Checks if a subcategory exists with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if it exists.</returns>
    public async Task<bool> IsExist(Guid id)
    {
        return await context.SubCategories.AsNoTracking().AnyAsync(x => x.Id == id);
    }

    /// <summary>
    /// Checks if a subcategory exists with a specific name within a specific store.
    /// Useful for name uniqueness validation within a store.
    /// </summary>
    /// <param name="storeId">The unique identifier of the store.</param>
    /// <param name="name">The name of the subcategory to check.</param>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if a match exists.</returns>
    public async Task<bool> IsExist(Guid storeId, string name)
    {
        return await context.SubCategories
            .AsNoTracking()
            .AnyAsync(su => su.StoreId == storeId && su.Name == name);
    }

    /// <summary>
    /// Checks if a subcategory with a specific ID exists within a specific store.
    /// </summary>
    /// <param name="storeId">The unique identifier of the store.</param>
    /// <param name="id">The unique identifier of the subcategory.</param>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if it exists in the store.</returns>
    public async Task<bool> IsExist(Guid storeId, Guid id)
    {
        return await context.SubCategories
            .AsNoTracking()
            .AnyAsync(su => su.StoreId == storeId && su.Id == id);
    }

    /// <summary>
    /// Retrieves a paged collection of all subcategories, ordered descending.
    /// </summary>
    /// <param name="page">The page number to retrieve (1-indexed).</param>
    /// <param name="length">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of subcategories.</returns>
    public async Task<ICollection<SubCategory>> getAllAsync(int page, int length)
    {
        return await context
            .SubCategories
            .AsNoTracking()
            .Skip((page - 1) * length)
            .OrderDescending()
            .Take(length)
            .ToICollectionAsync();
    }

    /// <summary>
    /// Adds a new subcategory to the database context.
    /// </summary>
    /// <param name="entity">The subcategory entity to add.</param>
    public void Add(SubCategory entity)
    {
        context.SubCategories.Add(entity);
    }

    /// <summary>
    /// Updates an existing subcategory in the database context.
    /// </summary>
    /// <param name="entity">The subcategory entity with updated values.</param>
    public void Update(SubCategory entity)
    {
        context.SubCategories.Update(entity);
    }

    /// <summary>
    /// Deletes a subcategory by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the subcategory to delete.</param>
    public void Delete(Guid id)
    {
        var subcategories = context.SubCategories.Where(su => su.Id == id)
            .ToICollection();
        context.SubCategories.RemoveRange(subcategories);
    }
}