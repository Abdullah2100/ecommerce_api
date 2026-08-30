using api.application;
using api.domain.entity;
using data.Interface;
using Microsoft.EntityFrameworkCore;

namespace data.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="PaymentType"/> entities.
/// Provides methods for adding, updating, and querying payment types.
/// </summary>
/// <param name="context">The database context used for data access.</param>
public class PaymentTypeRepository(AppDbContext context) : IPaymentTypeRepository
{
    /// <summary>
    /// Adds a new payment type to the database context.
    /// </summary>
    /// <param name="entity">The payment type entity to add.</param>
    public void Add(PaymentType entity)
    {
        context.PaymentTypes.Add(entity);
    }

    /// <summary>
    /// Updates an existing payment type in the database context.
    /// </summary>
    /// <param name="entity">The payment type entity with updated values.</param>
    public void Update(PaymentType entity)
    {
        context.PaymentTypes.Update(entity);
    }

    /// <summary>
    /// Retrieves a specific payment type by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the payment type.</param>
    /// <returns>A task representing the asynchronous operation, returning the payment type if found; otherwise, <c>null</c>.</returns>
    public async Task<PaymentType?> GetPaymentTypeGetPayment(Guid id)
    {
        return await context.PaymentTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <summary>
    /// Retrieves a paged collection of payment types without tracking changes.
    /// </summary>
    /// <param name="pageNum">The page number to retrieve (1-indexed).</param>
    /// <param name="pageSie">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of payment types.</returns>
    public async Task<ICollection<PaymentType>> GetPaymentTypes(int pageNum, int pageSie)
    {
        return await context.PaymentTypes.AsNoTracking()
            .Take(pageSie)
            .Skip((pageNum - 1) * pageSie)
            .ToListAsync();
    }

    /// <summary>
    /// Checks if a payment type with a specific name exists, excluding a specific identifier.
    /// Useful for uniqueness validation during updates.
    /// </summary>
    /// <param name="name">The name of the payment type to check.</param>
    /// <param name="id">The unique identifier to exclude from the check.</param>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if another payment type exists with that name.</returns>
    public async Task<bool> IsExistPaymentType(string name, Guid id)
    {
        return await context.PaymentTypes.AsNoTracking().AnyAsync(x => x.Name == name && x.Id != id);
    }

    /// <summary>
    /// Checks if a payment type exists with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if it exists; otherwise, <c>false</c>.</returns>
    public async Task<bool> IsExistPaymentType(Guid id)
    {
        return await context.PaymentTypes.AsNoTracking().AnyAsync(x => x.Id == id);
    }
}