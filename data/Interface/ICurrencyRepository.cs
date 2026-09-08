using api.domain.entity;
using data.dto.Response;

namespace data.Interface;

public interface ICurrencyRepository : IRepository<Currency>
{
    Task<Currency?> GetCurrencies(Guid id);
    Task<int> GetCurrenciesCount();
    Task<ICollection<CurrencyDto>> GetAll(int pageNum, int pageSize);
    Task Delete(Guid id);
    void Delete(ICollection<Currency> currencies);
    Task<bool> IsExist(string symbol);
}