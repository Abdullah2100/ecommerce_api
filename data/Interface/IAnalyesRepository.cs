
using data.dto.Response;

namespace data.Interface;

public interface IAnalyseRepository
{
    Task<AnalyzesOrderDto?> GetMonthAnalysis();
}