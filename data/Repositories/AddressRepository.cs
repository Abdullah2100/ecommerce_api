using api.application;
using api.domain.entity;
using data.dto.Response;
using data.Interface;
using data.mapper;
using data.util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace data.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="Address"/> entities.
/// </summary>
/// <param name="context">The database context used for data access.</param>
public class AddressRepository(
    AppDbContext context,
    ILogger<AddressRepository> logger) : IAddressRepository
{
    /// <summary>
    /// Adds a new address to the database context.
    /// </summary>
    /// <param name="entity">The address entity to add.</param>
    public async Task Add(Address entity)
    {
       await context.Address.AddAsync(entity);
    }

    /// <summary>
    /// Updates an existing address in the database context.
    /// </summary>
    /// <param name="entity">The address entity with updated values.</param>
    public void Update(Address entity)
    {
        context.Address.Update(entity);
    }

    /// <summary>
    /// Retrieves the total count of addresses associated with a specific owner.
    /// </summary>
    /// <param name="id">The unique identifier (OwnerId) of the owner.</param>
    /// <returns>A task representing the asynchronous operation, returning the count of addresses.</returns>
    public Task<int> GetAddressCount(Guid id)
    {
        var query = context
            .Address
            .AsNoTracking()
            .Where(ad => ad.OwnerId == id);

        ClsUtil.logSql<AddressRepository>(logger, query.ToQueryString());

        var addressCount = query
            .CountAsync();

        return addressCount;
    }

    /// <summary>
    /// Finds a specific address by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the address.</param>
    /// <returns>A task representing the asynchronous operation, returning the address if found; otherwise, null.</returns>
    public async Task<Address?> GetAddress(Guid id)
    {
        return await context.Address.FindAsync(id);
    }

    /// <summary>
    /// Retrieves the first address found for a given owner.
    /// </summary>
    /// <param name="id">The unique identifier of the owner.</param>
    /// <returns>A task representing the asynchronous operation, returning the address if found; otherwise, null.</returns>
    public async Task<Address?> GetAddressByOwnerId(Guid id)
    {
        var query = context
            .Address
            .AsNoTracking()
            .Where(x => x.OwnerId == id);

        ClsUtil.logSql<AddressRepository>(logger, query.ToQueryString());

        return await query.FirstOrDefaultAsync();
    }


    /// <summary>
    /// Marks all addresses of a specific owner as not being the current location.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner.</param>
    public async Task MakeAddressNotCurrentToId(Guid ownerId)
    {
        var query = context.Address.Where(ad => ad.OwnerId == ownerId);

        ClsUtil.logSql<AddressRepository>(logger, query.ToQueryString());

        if (!await query.AnyAsync()) return;

        await query.ExecuteUpdateAsync(value => value.SetProperty(a => a.IsCurrent, false));
    }

    /// <summary>
    /// Deletes a specific address by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the address to delete.</param>
    /// <exception cref="ArgumentNullException">Thrown if the address is not found.</exception>
    public void Delete(Guid id)
    {
        var query = context
            .Address
            .Where(x => x.Id == id);

        ClsUtil.logSql<AddressRepository>(logger, query.ToQueryString());


        if (!query.Any()) throw new ArgumentNullException();
        var entity = query.FirstOrDefault();
        if (entity == null) throw new ArgumentNullException();
        context.Address.Remove(entity);
    }
}