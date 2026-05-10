using api.application.Result;
using api.Presentation.dto.Response;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Interface;

public interface IAnalyseServices
{
    Task<IActionResult?> GetMonthAnalysis(Guid adminId);
}