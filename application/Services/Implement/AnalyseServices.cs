using api.application.Services.Interface;
using api.Infrastructure;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;

namespace api.application.Services.Implement;

public class AnalyseServices(
    IUnitOfWork unitOfWork,
    HybridCache cache) : IAnalyseServices
{
    public async Task<IActionResult> GetMonthAnalysis(Guid userId)
    {
        var user = await unitOfWork.UserRepository.GetUser(userId);

        var validationResult = user.IsValidateFunc();

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var result = await cache.GetOrCreateAsync(MemoryCacheKeys.AnalyseKey + '/' + userId, async ct =>
            {
                var analysis = await unitOfWork.AnalyseRepository.GetMonthAnalysis();
                return analysis;
            },
            tags: [MemoryCacheKeys.AnalyseKey]);

        if (result is null)
        {
            return new ObjectResult("Could not calculate analyzes")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }
        
        
        return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
    }
}