using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services.Interface;

public interface ICurrencyServices
{
    Task<IActionResult> CreateCurrency(Guid adminId, CreateCurrencyDto currencyDto);
    Task<IActionResult> UpdateCurrency(Guid adminId, UpdateCurrencyDto currencyDto);
    Task<IActionResult> DeleteCurrency(Guid adminId, Guid id);
    Task<IActionResult> GetCurrency(int page = 1, int pageSize = 10);
}