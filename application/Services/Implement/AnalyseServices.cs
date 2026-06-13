using api.application.Services.Interface;
using api.Infrastructure;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;

namespace api.application.Services.Implement;

public class AnalyseServices(
    IUnitOfWork unitOfWork,
    HybridCache cache,
    ILogger<AnalyseServices> logger) : IAnalyseServices
{
    public async Task<IActionResult> GetMonthAnalysis(Guid userId)
    {
        logger.LogInformation("Start calling getMonthAnalysis from dashboard by {userId}", userId);
        var user = await unitOfWork.UserRepository.GetUser(userId);

        var validationResult = user.IsValidateFunc();

        if (validationResult is not null)
        {
            logger.LogWarning("validation error at  getMonthAnalysis from userId  {userId}",userId);

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
            logger.LogError("there is no result from  getMonthAnalysis from dashboard by {userId}",userId);

            return new ObjectResult("Could not calculate analyzes")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        logger.LogInformation("end calling getMonthAnalysis from dashboard by {userId}", userId);


        return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
    }
}