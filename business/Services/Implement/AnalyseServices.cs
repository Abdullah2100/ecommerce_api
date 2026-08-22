using api.application;
using api.Infrastructure;
using api.util;
using business.mapper;
using business.Services.Interface;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace business.Services.Implement;

public class AnalyseServices(
    IUnitOfWork unitOfWork,
    HybridCache cache,
    ILogger<AnalyseServices> logger) : IAnalyseServices
{
    public async Task<Result> GetMonthAnalysis(Guid userId)
    {
        logger.LogInformation("Start calling getMonthAnalysis ");
        var user = await unitOfWork.UserRepository.GetUser(userId);

        var validationResult = user.IsValidateFunc();

        if (validationResult is not null)
        {
            logger.LogInformation("user not valid {userId} validationError {message}", userId, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var result = await cache.GetOrCreateAsync(MemoryCacheKeys.AnalyseKey + '/' + userId, async ct =>
            {
                var analysis = await unitOfWork.AnalyseRepository.GetMonthAnalysis();
                return analysis;
            },
            tags: [MemoryCacheKeys.AnalyseKey]);

        if (result is null)
        {
            logger.LogError("there is no data to calculate month analyse");
            return new Result(false, "Could not calculate analyzes", null, 500);
        }

        logger.LogInformation("end calling getMonthAnalysis");
        return new Result(true, null, result, 200);
    }
}