using api.application;
using data.dto.Request;

namespace api.application.Services.Interface;

public interface ICurrencyServices
{
    Task<Result> CreateCurrency(Guid adminId, CreateCurrencyDto currencyDto);
    Task<Result> UpdateCurrency(Guid adminId, UpdateCurrencyDto currencyDto);
    Task<Result> DeleteCurrency(Guid adminId, Guid id);
    Task<Result> GetCurrency(int page = 1, int pageSize = 10);
}