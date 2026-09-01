using api.application;
using api.domain.entity;
using data.Interface;
using data.util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace data.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="Currency"/> entities.
/// </summary>
/// <param name="context">The database context used for data access.</param>
public class CurrencyRepository(
    AppDbContext context,
    ILogger<CurrencyRepository> logger) : ICurrencyRepository
{
    /// <summary>
    /// Tracks a new currency entity to be added to the database.
    /// </summary>
    /// <param name="entity">The currency entity to add.</param>
    public void Add(Currency entity)
    {
        context.Add(entity);
    }

    /// <summary>
    /// Updates an existing currency entity in the database context.
    /// </summary>
    /// <param name="entity">The currency entity with updated values.</param>
    public void Update(Currency entity)
    {
        context.Update(entity);
    }

    /// <summary>
    /// Retrieves a specific currency by its unique identifier from the Payments collection.
    /// </summary>
    /// <param name="id">The unique identifier of the currency.</param>
    /// <returns>A task representing the asynchronous operation, returning the currency or null if not found.</returns>
    public async Task<Currency?> GetCurrencies(Guid id)
    {
        var element = await context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        return element;
    }

    /// <summary>
    /// Retrieves a specified number of currencies in a random order from the Currencies collection.
    /// </summary>
    /// <param name="randomNumber">The number of currencies to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of random currencies.</returns>
    public async Task<ICollection<Currency>> GetCurrencies(int randomNumber)
    {
        var query = context
                .Currencies
                .AsNoTracking()
                .OrderBy(x => Guid.NewGuid())
                .Take(randomNumber)
            ;

        ClsUtil.logSql<CurrencyRepository>(logger, query.ToQueryString());

        return await query.ToListAsync();
    }

    /// <summary>
    /// Gets the total number of currencies in the Currencies collection.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, returning the total count.</returns>
    public async Task<int> GetCurrenciesCount()
    {
        var query = context
            .Currencies
            .AsNoTracking();

        ClsUtil.logSql<CurrencyRepository>(logger, query.ToQueryString());

        return await query.CountAsync();
    }

    /// <summary>
    /// Retrieves a paged collection of currencies from the Payments collection.
    /// </summary>
    /// <param name="pageNum">The page number to retrieve (1-indexed).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of currencies.</returns>
    public async Task<ICollection<Currency>> GetAll(int pageNum, int pageSize)
    {
        var query = context.Payments
                .AsNoTracking()
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
            ;
        ClsUtil.logSql<CurrencyRepository>(logger, query.ToQueryString());

        return await query.ToListAsync();
    }

    /// <summary>
    /// Deletes a specific currency by its identifier from the Payments collection.
    /// </summary>
    /// <param name="id">The unique identifier of the currency to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Delete(Guid id)
    {
        var query = context
            .Payments
            .AsNoTracking()
            .Where(x => x.Id == id);
        ClsUtil.logSql<CurrencyRepository>(logger, query.ToQueryString());

        if (! await query.AnyAsync()) return;
        context.Payments.RemoveRange(query);
    }

    /// <summary>
    /// Deletes a collection of currency entities from the Currencies collection.
    /// </summary>
    /// <param name="currencies">The collection of currencies to remove.</param>
    public void Delete(ICollection<Currency> currencies)
    {
        context.Currencies.RemoveRange(currencies);
    }

    /// <summary>
    /// Checks if a currency exists with the specified symbol in the Payments collection.
    /// </summary>
    /// <param name="symbol">The currency symbol (e.g., "$", "€") to check.</param>
    /// <returns>A task representing the asynchronous operation, returning true if it exists; otherwise, false.</returns>
    public async Task<bool> isExist(string symbol)
    {
        var query =  context.Payments
            .AsNoTracking()
            .Where(x => x.Symbol == symbol);
        ClsUtil.logSql<CurrencyRepository>(logger, query.ToQueryString());
 
        return await  query.AnyAsync();
    }
}