using api.application;
using api.domain.entity;
using data.Interface;
using data.util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace data.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="Banner"/> entities.
/// </summary>
/// <param name="context">The database context used for data access.</param>
public class BannerRepository(
    AppDbContext context,
    ILogger<BannerRepository> logger) : IBannerRepository
{
    /// <summary>
    /// Tracks a new banner entity to be added to the database.
    /// </summary>
    /// <param name="entity">The banner entity to add.</param>
    public void Add(Banner entity)
    {
        context
            .Banner
            .AddAsync(entity);
    }

    /// <summary>
    /// Updates an existing banner entity in the database context.
    /// </summary>
    /// <param name="entity">The banner entity with updated values.</param>
    public void Update(Banner entity)
    {
        context
            .Banner
            .Update(entity);
    }

    /// <summary>
    /// Gets the total number of banners in the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, returning the total count.</returns>
    public Task<int> GetBannerCount()
    {
        var query = context
            .Banner
            .AsNoTracking();

        ClsUtil.logSql<BannerRepository>(logger, query.ToQueryString());
        return query.CountAsync();
    }

    /// <summary>
    /// Gets the count of active banners for a specific store.
    /// A banner is considered active if it was created within the last hour.
    /// </summary>
    /// <param name="storeId">The unique identifier of the store.</param>
    /// <returns>A task representing the asynchronous operation, returning the count of active banners.</returns>
    public Task<int> GetBannerCount(Guid storeId)
    {
        var query = context.Banner
            .AsNoTracking()
            .Where(ba => ba.StoreId == storeId && ba.CreatedAt.AddHours(1) >= DateTime.Now);

        ClsUtil.logSql<BannerRepository>(logger, query.ToQueryString());
        return query.CountAsync();
    }

    /// <summary>
    /// Deletes a banner by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the banner to delete.</param>
    public void Delete(Guid id)
    {
        var query = context
            .Banner
            .AsNoTracking()
            .Where(ba => ba.Id == id);
        ClsUtil.logSql<BannerRepository>(logger, query.ToQueryString());

        if (!query.Any()) return;

        context.Remove(query);
    }

    /// <summary>
    /// Deletes a collection of banner entities.
    /// </summary>
    /// <param name="banners">The collection of banners to remove.</param>
    public void Delete(ICollection<Banner> banners)
    {
        context.Banner.RemoveRange(banners);
    }

    /// <summary>
    /// Retrieves a specific banner by its identifier without tracking changes.
    /// </summary>
    /// <param name="id">The unique identifier of the banner.</param>
    /// <returns>A task representing the asynchronous operation, returning the banner or null if not found.</returns>
    public async Task<Banner?> GetBanner(Guid id)
    {
        var query = context
            .Banner
            .AsNoTracking()
            .Where(ba => ba.Id == id);
        ClsUtil.logSql<BannerRepository>(logger, query.ToQueryString());

        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves a banner by its identifier and store identifier without tracking changes.
    /// </summary>
    /// <param name="id">The unique identifier of the banner.</param>
    /// <param name="storeId">The unique identifier of the store owner.</param>
    /// <returns>A task representing the asynchronous operation, returning the banner or null if not found.</returns>
    public async Task<Banner?> GetBanner(Guid id, Guid storeId)
    {
        var query = context
            .Banner
            .AsNoTracking()
            .Where(ba => ba.Id == id && ba.StoreId == storeId);
        ClsUtil.logSql<BannerRepository>(logger, query.ToQueryString());

        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves a paged collection of banners for a specific store, ordered by creation date descending.
    /// </summary>
    /// <param name="id">The unique identifier of the store.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of banners.</returns>
    public async Task<ICollection<Banner>> GetBanners(Guid id, int pageNumber, int pageSize)
    {
        var query = context.Banner
            .AsNoTracking()
            .Where(ba => ba.StoreId == id)
            .OrderByDescending(ba => ba.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        ClsUtil.logSql<BannerRepository>(logger, query.ToQueryString());

        return await query.ToListAsync();
    }

    /// <summary>
    /// Retrieves a paged collection of all banners, ordered by creation date descending.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of banners.</returns>
    public async Task<ICollection<Banner>> GetBanners(int pageNumber, int pageSize)
    {
        var query = context.Banner
            .AsNoTracking()
            .OrderByDescending(ba => ba.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        ClsUtil.logSql<BannerRepository>(logger, query.ToQueryString());

        return await query.ToListAsync();
    }

    /// <summary>
    /// Retrieves a specified number of banners, ordered by ID.
    /// </summary>
    /// <param name="randomLength">The number of banners to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of banners.</returns>
    public async Task<ICollection<Banner>> GetBanners(int randomLength)
    {
        var query = context.Banner
            .AsNoTracking()
            .OrderBy(ba => ba.Id)
            .Take(randomLength);
        ClsUtil.logSql<BannerRepository>(logger, query.ToQueryString());

        return await query.ToListAsync();
    }

    /// <summary>
    /// Retrieves a specified number of "not active" banners based on creation date age.
    /// </summary>
    /// <param name="randomLength">The number of banners to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of banners.</returns>
    public async Task<ICollection<Banner>> GetNotActiveBanners(int randomLength)
    {
        var query = context.Banner
                .AsNoTracking()
                .Where(ba => (ba.CreatedAt-DateTime.UtcNow).Days > 2)
                .OrderBy(ba => ba.Id)
                .Take(randomLength)
            ;
        ClsUtil.logSql<BannerRepository>(logger, query.ToQueryString());

        return await query.ToListAsync();
    }
}