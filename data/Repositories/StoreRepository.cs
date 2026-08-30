using api.application;
using api.domain.entity;
using data.Interface;
using Microsoft.EntityFrameworkCore;

namespace data.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="Store"/> entities.
/// Provides methods for store registration, updates, and complex querying including related addresses and users.
/// </summary>
/// <param name="context">The database context used for data access.</param>
public class StoreRepository(AppDbContext context) : IStoreRepository
{
    /// <summary>
    /// Adds a new store to the database context.
    /// </summary>
    /// <param name="entity">The store entity to add.</param>
    public void Add(Store entity)
    {
        context.Stores.Add(entity);
    }

    /// <summary>
    /// Updates an existing store in the database context.
    /// Creates a new store instance to ensure only specific fields are updated.
    /// </summary>
    /// <param name="entity">The store entity containing updated values.</param>
    public void Update(Store entity)
    {
        var storeData = new Store()
        {
            Id = entity.Id,
            Name = entity.Name,
            WallpaperImage = entity.WallpaperImage,
            SmallImage = entity.SmallImage,
            IsBlock = entity.IsBlock,
            UserId = entity.UserId,
            UpdatedAt = entity.UpdatedAt
        };
        context.Stores.Update(storeData);
    }

    /// <summary>
    /// Toggles the blocked status of a store by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the store.</param>
    /// <exception cref="ArgumentNullException">Thrown when the store is not found.</exception>
    public void Delete(Guid id)
    {
        var store = context.Stores.Find(id);
        if (store == null) throw new ArgumentNullException();
        store.IsBlock = !store.IsBlock;
    }

    /// <summary>
    /// Retrieves a specific store by its identifier, including user info and associated addresses.
    /// </summary>
    /// <param name="id">The unique identifier of the store.</param>
    /// <returns>A task representing the asynchronous operation, returning the store or <c>null</c> if not found.</returns>
    public async Task<Store?> GetStore(Guid id)
    {
        var store = await context
            .Stores
            .Include(st => st.user)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(st => st.Id == id);

        if (store is null) return null;

        store.Addresses = await context
            .Address
            .AsNoTracking()
            .Where(ad => ad.OwnerId == store.Id)
            .ToListAsync();
        return store;
    }

    /// <summary>
    /// Retrieves a store associated with a specific user ID.
    /// </summary>
    /// <param name="id">The unique identifier of the user (owner).</param>
    /// <returns>A task representing the asynchronous operation, returning the store or <c>null</c> if not found.</returns>
    public async Task<Store?> GetStoreByUserId(Guid id)
    {
        Store? store = await context
            .Stores
            .Include(st => st.user)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(st => st.UserId == id);

        if (store is null) return null;

        store.Addresses = await context
            .Address
            .AsNoTracking()
            .Where(ad => ad.OwnerId == store.Id)
            .ToListAsync();
        return store;
    }

    /// <summary>
    /// Retrieves a collection of stores whose names start with the specified prefix.
    /// </summary>
    /// <param name="prefix">The name prefix to search for.</param>
    /// <param name="length">The maximum number of stores to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of matching stores.</returns>
    public async Task<ICollection<Store>> GetStores(string prefix, int length)
    {
        var stores = await context
            .Stores
            .AsNoTracking()
            .Include(st => st.user)
            .AsSplitQuery()
            .Where(x => x.Name.StartsWith(prefix))
            .Take(length)
            .ToListAsync();

        foreach (var store in stores)
        {
            store.Addresses = await context
                .Address
                .AsNoTracking()
                .Where(ad => ad.OwnerId == store.Id)
                .ToListAsync();
        }

        return stores;
    }

    /// <summary>
    /// Retrieves a paged collection of stores, including their subcategories and addresses.
    /// </summary>
    /// <param name="page">The page number to retrieve (1-indexed).</param>
    /// <param name="length">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of stores.</returns>
    public async Task<ICollection<Store>> GetStores(int page, int length)
    {
        ICollection<Store> stores = await context
            .Stores
            .Include(st => st.user)
            .Include(st => st.SubCategories)
            .AsSplitQuery()
            .AsNoTracking()
            .Skip((page - 1) * length)
            .Take(length)
            .ToListAsync();

        if (stores.Count <= 0) return new List<Store>();

        foreach (var store in stores)
        {
            store.Addresses = await context
                .Address
                .AsNoTracking()
                .Where(ad => ad.OwnerId == store.Id)
                .ToListAsync();
        }

        return stores;
    }

    /// <summary>
    /// Calculates the total number of pages available for all stores based on the items per page.
    /// </summary>
    /// <param name="storePerPage">The number of stores per page.</param>
    /// <returns>A task representing the asynchronous operation, returning the total page count.</returns>
    public async Task<int> GetStoresCount(int storePerPage)
    {
        var count = await context
            .Stores
            .AsNoTracking()
            .CountAsync();
        if (count == 0) return 0;
        count = (int)Math.Ceiling((double)count / storePerPage);
        return count;
    }

    /// <summary>
    /// Checks if a store exists with the specified name.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if it exists.</returns>
    public async Task<bool> IsExist(string name)
    {
        return await context
            .Stores
            .AsNoTracking()
            .AnyAsync(st => st.Name == name);
    }

    /// <summary>
    /// Checks if a store exists with the specified name, excluding a specific identifier.
    /// Useful for name uniqueness validation during updates.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <param name="id">The unique identifier to exclude.</param>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if another store exists with that name.</returns>
    public async Task<bool> IsExist(string name, Guid id)
    {
        return await context
            .Stores
            .AsNoTracking()
            .AnyAsync(st => st.Name == name && st.Id != id);
    }

    /// <summary>
    /// Checks if a store exists with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if it exists.</returns>
    public async Task<bool> IsExist(Guid id)
    {
        return await context
            .Stores
            .AsNoTracking()
            .AnyAsync(st => st.Id == id);
    }

    /// <summary>
    /// Checks if a store exists and contains a specific subcategory.
    /// </summary>
    /// <param name="id">The unique identifier of the store.</param>
    /// <param name="subCategoryId">The unique identifier of the subcategory.</param>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if the store contains the subcategory.</returns>
    public async Task<bool> IsExist(Guid id, Guid subCategoryId)
    {
        return await context
            .Stores
            .Include(st => st.SubCategories)
            .AsNoTracking()
            .AnyAsync(st =>
                st.Id == id &&
                st.SubCategories.Any(sc => sc.Id == subCategoryId) != false);
    }
}