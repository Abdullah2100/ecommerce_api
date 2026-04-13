using api.Presentation.dto;
using api.Presentation.dto.Response;

namespace api.domain.Interface;

public interface IAnalyseRepository
{
    Task<AnalyzesOrderDto?> GetMonthAnalysis();
}