using api.application.Interface;
using api.Infrastructure;
using api.shared.mapper;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services;

public class AnalyseServices(IUnitOfWork unitOfWork) : IAnalyseServices
{
    public async Task<IActionResult> GetMonthAnalysis(Guid adminId)
    {
        var user = await unitOfWork.UserRepository.GetUser(adminId);

        var validationResult = user.IsValidateFunc();

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var result = await unitOfWork.AnalyseRepository.GetMonthAnalysis();

        if (result is null)
        {
            return new ObjectResult("Could not calculate analyzes")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }


        return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
    }
}