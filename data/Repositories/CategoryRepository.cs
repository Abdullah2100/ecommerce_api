using api.application;
using api.domain.entity;
using data.Interface;
using Microsoft.EntityFrameworkCore;

namespace data.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="Category"/> entities.
/// </summary>
/// <param name="context">The database context used for data access.</param>
public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    /// <summary>
    /// Tracks a new category entity to be added to the database.
    /// </summary>
    /// <param name="entity">The category entity to add.</param>
    public void Add(Category entity)
    {
        context.Categories.Add(entity);
    }

    /// <summary>
    /// Updates an existing category entity in the database context.
    /// </summary>
    /// <param name="entity">The category entity with updated values.</param>
    public void Update(Category entity)
    {
        context.Categories.Update(entity);
    }

    /// <summary>
    /// Retrieves a specific category by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the category.</param>
    /// <returns>A task representing the asynchronous operation, returning the category or null if not found.</returns>
    public async Task<Category?> GetCategory(Guid id)
    {
        return await context.Categories.FindAsync(id);
    }

    /// <summary>
    /// Retrieves a paged collection of categories, ordered descending.
    /// </summary>
    /// <param name="page">The page number (1-indexed).</param>
    /// <param name="length">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of categories.</returns>
    public async Task<ICollection<Category>> GetCategories(int page, int length)
    {
        return await context
            .Categories
            .AsNoTracking()
            .Skip((page - 1) * length)
            .Take(length)
            .OrderDescending()
            .ToListAsync();
    }

    /// <summary>
    /// Gets the total number of categories in the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, returning the total count.</returns>
    public async Task<int> GetCategoriesCount()
    {
        return await context
            .Categories
            .AsNoTracking()
            .CountAsync();
    }

    /// <summary>
    /// Retrieves a specified number of categories in a random order.
    /// </summary>
    /// <param name="randomNumber">The number of categories to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of categories.</returns>
    public async Task<ICollection<Category>> GetCategories(int randomNumber)
    {
        return await context
            .Categories
            .AsNoTracking()
            .OrderBy(x => Guid.NewGuid())
            .Take(randomNumber)
            .ToListAsync();
    }

    /// <summary>
    /// Checks if a category exists with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>A task representing the asynchronous operation, returning true if it exists; otherwise, false.</returns>
    public async Task<bool> IsExist(Guid id)
    {
        return await context
            .Categories
            .AsNoTracking()
            .AnyAsync(e => e.Id == id);
    }

    /// <summary>
    /// Checks if a category exists with the specified name.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>A task representing the asynchronous operation, returning true if it exists; otherwise, false.</returns>
    public async Task<bool> IsExist(string name)
    {
        return await context
            .Categories
            .AsNoTracking()
            .AnyAsync(e => e.Name == name);
    }

    /// <summary>
    /// Checks if a category exists with the specified name, excluding a specific identifier.
    /// Useful for uniqueness validation during updates.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <param name="id">The unique identifier to exclude from the check.</param>
    /// <returns>A task representing the asynchronous operation, returning true if another category exists with that name.</returns>
    public async Task<bool> IsExist(string name, Guid id)
    {
        return await context
            .Categories
            .AsNoTracking()
            .AnyAsync(e => e.Name == name && e.Id != id);
    }

    /// <summary>
    /// Deletes a category by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the category to delete.</param>
    /// <exception cref="ArgumentNullException">Thrown when the category is not found.</exception>
    public void Delete(Guid id)
    {
        var category = context
            .Categories
            .AsNoTracking()
            .FirstOrDefault(ca => ca.Id == id);
        if (category is null) throw new ArgumentNullException();
        context.Categories.Remove(category);
    }

    /// <summary>
    /// Deletes a collection of category entities.
    /// </summary>
    /// <param name="categories">The collection of categories to remove.</param>
    public void Delete(ICollection<Category> categories)
    {
        context.Categories.RemoveRange(categories);
    }
}