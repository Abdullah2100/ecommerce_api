using api.application;

namespace business.Services.Interface;

public interface IAnalyseServices
{
    Task<Result> GetMonthAnalysis(Guid userId);
}