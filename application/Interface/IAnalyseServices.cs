using api.application.Result;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface IAnalyseServices
{
    Task<AnalyzesOrderDto?> GetMonthAnalysis(Guid adminId);
}