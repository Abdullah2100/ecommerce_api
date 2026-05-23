using Microsoft.AspNetCore.Mvc;

namespace api.application.Services.Interface;

public interface IAnalyseServices
{
    Task<IActionResult> GetMonthAnalysis(Guid userId);
}