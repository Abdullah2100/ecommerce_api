using api.application;
using api.domain.entity;
using data.Interface;
using Microsoft.EntityFrameworkCore;

namespace data.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="Address"/> entities.
/// </summary>
/// <param name="context">The database context used for data access.</param>
public class AddressRepository(AppDbContext context) : IAddressRepository
{
    /// <summary>
    /// Adds a new address to the database context.
    /// </summary>
    /// <param name="entity">The address entity to add.</param>
    public void Add(Address entity)
    {
        context.Address.Add(entity);
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
        return context
            .Address
            .AsNoTracking()
            .Where(ad => ad.OwnerId == id)
            .CountAsync();
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
        return await context
            .Address
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OwnerId == id);
    }

    /// <summary>
    /// Retrieves all addresses associated with a specific owner.
    /// </summary>
    /// <param name="id">The unique identifier of the owner.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of addresses.</returns>
    public async Task<ICollection<Address>> GetAllAddressByOwnerId(Guid id)
    {
        return await context
            .Address
            .AsNoTracking()
            .Where(x => x.OwnerId == id)
            .ToListAsync();
    }

    /// <summary>
    /// Sets a specific address as the current (active) location for an owner.
    /// </summary>
    /// <param name="id">The unique identifier of the address.</param>
    /// <param name="ownerId">The unique identifier of the owner.</param>
    /// <exception cref="ArgumentNullException">Thrown if the address is not found.</exception>
    public void UpdateCurrentLocation(Guid id, Guid ownerId)
    {
        var currentAddress = context.Address
            .AsNoTracking()
            .FirstOrDefault(ad => ad.OwnerId == ownerId && ad.Id == id);
        if (currentAddress == null) throw new ArgumentNullException();
        currentAddress.IsCurrent = true;
    }

    /// <summary>
    /// Marks all addresses of a specific owner as not being the current location.
    /// </summary>
    /// <param name="ownerId">The unique identifier of the owner.</param>
    public void MakeAddressNotCurrentToId(Guid ownerId)
    {
        var address = context.Address
            .Where(ad => ad.OwnerId == ownerId);
        foreach (var currentAddress in address)
        {
            currentAddress.IsCurrent = false;
        }

        context.UpdateRange(address);
    }

    /// <summary>
    /// Deletes a specific address by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the address to delete.</param>
    /// <exception cref="ArgumentNullException">Thrown if the address is not found.</exception>
    public void Delete(Guid id)
    {
        var address = context
            .Address
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
        if (address is null) throw new ArgumentNullException();
        context.Remove(address);
    }
}