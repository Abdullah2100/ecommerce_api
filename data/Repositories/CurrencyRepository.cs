using api.application;
using api.domain.entity;
using data.Interface;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Repositories;

public class CurrencyRepository(AppDbContext context) : ICurrencyRepository
{
    public void Add(Currency entity)
    {
        context.Add(entity);
    }

    public void Update(Currency entity)
    {
        context.Update(entity);
    }

    public async Task<Currency?> GetCurrencies(Guid id)
    {
        Currency? element = await context.Payments
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        return element;
    }

    public Task<ICollection<Currency>> GetCurrencies(int randomNumber)
    {
        return context.Currencies
            .OrderBy(x => Guid.NewGuid())
            .Take(randomNumber)
            .ToICollectionAsync();
    }

    public Task<int> GetCurrenciesCount()
    {
        return context.Currencies.CountAsync();
    }

    public async Task<ICollection<Currency>> GetAll(int pageNum, int pageSize)
    {
        return await context.Payments
            .AsNoTracking()
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToICollectionAsync();
    }

    public async Task Delete(Guid id)
    {
        Currency? element = await context.Payments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (element is null) return;
        context.Payments.Remove(element);
    }

    public void Delete(ICollection<Currency> currencies)
    {
        context.Currencies.RemoveRange(currencies);
    }

    public async Task<bool> isExist(string symbol)
    {
        return await context.Payments
            .AsNoTracking()
            .AnyAsync(x => x.Symbol == symbol);
    }
}