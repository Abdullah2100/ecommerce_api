 
using api.application;
using api.domain.entity;
using data.Interface;
using data.util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace data.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="GeneralSetting"/> entities.
/// </summary>
/// <param name="context">The database context used for data access.</param>
/// <param name="logger">The logger used to log generated SQL queries.</param>
public class GeneralSettingRepository(
    AppDbContext context,
    ILogger<GeneralSettingRepository> logger
) : IGeneralSettingRepository
{
    /// <summary>
    /// Retrieves a paged collection of general settings without tracking changes.
    /// </summary>
    /// <param name="page">The page number to retrieve (1-indexed).</param>
    /// <param name="length">The number of items per page.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning a collection
    /// of general settings.
    /// </returns>
    public async Task<ICollection<GeneralSetting>> Getgenralsettings(
        int page,
        int length
    )
    {
        var query = context
            .GeneralSettings
            .AsNoTracking()
            .Skip((page - 1) * length)
            .Take(length);

        ClsUtil.logSql<GeneralSettingRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.ToListAsync();
    }

    /// <summary>
    /// Tracks a new general setting entity to be added to the database.
    /// </summary>
    /// <param name="entity">The general setting entity to add.</param>
    public void Add(GeneralSetting entity)
    {
        context.GeneralSettings.Add(entity);
    }

    /// <summary>
    /// Updates an existing general setting entity in the database context.
    /// </summary>
    /// <param name="entity">The general setting entity with updated values.</param>
    public void Update(GeneralSetting entity)
    {
        context.GeneralSettings.Update(entity);
    }

    /// <summary>
    /// Deletes a general setting by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the general setting to delete.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the general setting is not found.
    /// </exception>
    public void Delete(Guid id)
    {
        var query = context
            .GeneralSettings
            .AsNoTracking()
            .Where(gs => gs.Id == id);

        ClsUtil.logSql<GeneralSettingRepository>(
            logger,
            query.ToQueryString()
        );

        var generalSetting = query.FirstOrDefault();

        if (generalSetting == null)
            throw new ArgumentNullException();

        context.Remove(generalSetting);
    }

    /// <summary>
    /// Retrieves a specific general setting by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the general setting.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning the general
    /// setting if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<GeneralSetting?> GetGeneralSetting(Guid id)
    {
        var query = context
            .GeneralSettings
            .AsNoTracking()
            .Where(gs => gs.Id == id);

        ClsUtil.logSql<GeneralSettingRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Checks whether a general setting exists with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning
    /// <c>true</c> if the setting exists; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> IsExist(Guid id)
    {
        var query = context
            .GeneralSettings
            .AsNoTracking()
            .Where(gs => gs.Id == id);

        ClsUtil.logSql<GeneralSettingRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.AnyAsync();
    }

    /// <summary>
    /// Checks whether a general setting exists with the specified name.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning
    /// <c>true</c> if the setting exists; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> IsExist(string name)
    {
        var query = context
            .GeneralSettings
            .AsNoTracking()
            .Where(gs => gs.Name == name);

        ClsUtil.logSql<GeneralSettingRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.AnyAsync();
    }

    /// <summary>
    /// Checks whether a general setting exists with the specified identifier
    /// and name.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <param name="name">The name to check.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning
    /// <c>true</c> if a matching setting exists; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> IsExist(Guid id, string name)
    {
        var query = context
            .GeneralSettings
            .AsNoTracking()
            .Where(gs => gs.Id == id && gs.Name == name);

        ClsUtil.logSql<GeneralSettingRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.AnyAsync();
    }
}