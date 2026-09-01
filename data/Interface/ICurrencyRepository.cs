using api.domain.entity;

namespace data.Interface;

public interface ICurrencyRepository : IRepository<Currency>
{
    Task<Currency?> GetCurrencies(Guid id);
    Task<ICollection<Currency>> GetCurrencies(int randomNumber);
    Task<int> GetCurrenciesCount();
    Task<ICollection<Currency>> GetAll(int pageNum, int pageSize);
    Task Delete(Guid id);
    void Delete(ICollection<Currency> currencies);
    Task<bool> isExist(string symbol);
}