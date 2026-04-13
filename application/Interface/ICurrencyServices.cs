using api.Presentation.dto;
using api.application.Result;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface ICurrencyServices
{
    Task<Result<CurrencyDto?>> CreateCurrency(Guid adminId,CreateCurrencyDto currencyDto);
    Task<Result<CurrencyDto?>> UpdateCurrency(Guid adminId,UpdateCurrencyDto currencyDto);
    Task<Result<bool>> DeleteCurrency(Guid adminId,Guid id);
    Task<Result<List<CurrencyDto>>> GetCurrency(int page = 1, int pageSize = 10);
}