using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface IAnalyseServices
{
    Task<Result<AnalyzesOrderDto?>> GetMonthAnalysis(Guid adminId);
}