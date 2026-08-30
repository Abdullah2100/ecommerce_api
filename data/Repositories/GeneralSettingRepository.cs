using api.application;
using api.domain.entity;
using data.Interface;
using Microsoft.EntityFrameworkCore;

namespace data.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="GeneralSetting"/> entities.
/// </summary>
/// <param name="context">The database context used for data access.</param>
public class GeneralSettingRepository(AppDbContext context) : IGeneralSettingRepository
{
    /// <summary>
    /// Retrieves a paged collection of general settings without tracking changes.
    /// </summary>
    /// <param name="page">The page number to retrieve (1-indexed).</param>
    /// <param name="length">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of general settings.</returns>
    public async Task<ICollection<GeneralSetting>> Getgenralsettings(int page, int length)
    {
        return await context
            .GeneralSettings
            .AsNoTracking()
            .Skip((page - 1) * length)
            .Take(length)
            .ToListAsync();
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
    /// <exception cref="ArgumentNullException">Thrown when the general setting is not found.</exception>
    public void Delete(Guid id)
    {
        var generalSetting = context.GeneralSettings
            .AsNoTracking()
            .FirstOrDefault(gs => gs.Id == id);
        if (generalSetting == null) throw new ArgumentNullException();

        context.Remove(generalSetting);
    }

    /// <summary>
    /// Retrieves a specific general setting by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the general setting.</param>
    /// <returns>A task representing the asynchronous operation, returning the general setting if found; otherwise, null.</returns>
    public async Task<GeneralSetting?> GetGeneralSetting(Guid id)
    {
        return await context.GeneralSettings.FindAsync(id);
    }

    /// <summary>
    /// Checks if a general setting exists with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>A task representing the asynchronous operation, returning true if it exists; otherwise, false.</returns>
    public async Task<bool> IsExist(Guid id)
    {
        return await context.GeneralSettings.FindAsync(id) != null;
    }

    /// <summary>
    /// Checks if a general setting exists with the specified name.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>A task representing the asynchronous operation, returning true if it exists; otherwise, false.</returns>
    public async Task<bool> IsExist(string name)
    {
        return await context
            .GeneralSettings
            .AsNoTracking()
            .AnyAsync(gs => gs.Name == name);
    }

    /// <summary>
    /// Checks if a general setting exists with the specified identifier and name.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <param name="name">The name to check.</param>
    /// <returns>A task representing the asynchronous operation, returning true if a matching setting exists.</returns>
    public async Task<bool> IsExist(Guid id, string name)
    {
        return await context
            .GeneralSettings
            .AsNoTracking()
            .AnyAsync(gs => gs.Id == id && gs.Name == name);
    }
}