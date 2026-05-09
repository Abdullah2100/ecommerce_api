using api.Presentation.dto;
using api.application.Result;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface ICurrencyServices
{
    Task<CurrencyDto?> CreateCurrency(Guid adminId,CreateCurrencyDto currencyDto);
    Task<CurrencyDto?> UpdateCurrency(Guid adminId,UpdateCurrencyDto currencyDto);
    Task<bool> DeleteCurrency(Guid adminId,Guid id);
    Task<List<CurrencyDto>> GetCurrency(int page = 1, int pageSize = 10);
}