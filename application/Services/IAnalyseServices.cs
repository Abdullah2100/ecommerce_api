using api.application.Interface;
using api.application.Result;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Response;
using api.shared.mapper;

namespace api.application.Services;

public class AnalyseServices(IUnitOfWork unitOfWork):IAnalyseServices
{
    public async Task<Result<AnalyzesOrderDto?>> GetMonthAnalysis(Guid adminId)
    {
        User? user = await unitOfWork.UserRepository.GetUser(adminId);
        
        var isValid = user.IsValidateFunc();
        if (isValid is not null)
        {
            return new Result<AnalyzesOrderDto?>(
                data: null,
                message: isValid.Message,
                isSuccessful: false,
                statusCode: isValid.StatusCode
            );
        }

        var  result = await unitOfWork.AnalyseRepository.GetMonthAnalysis();
        if (result is  null)
        {
            return new Result<AnalyzesOrderDto?>(
                data: null,
                message: "Could not calculate analayes",
                isSuccessful: false,
                statusCode:404 
            );
        }
        
        return new Result<AnalyzesOrderDto?>(
            data: result,
            message: null,
            isSuccessful: true,
            statusCode:200 
        );
    }


}