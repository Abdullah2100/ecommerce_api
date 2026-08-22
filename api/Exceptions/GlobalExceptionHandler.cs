using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace api.Exceptions;

public class GlobalExceptionHandler(IProblemDetailsService ps,ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        System.Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var endpoint = httpContext.GetEndpoint().DisplayName;
        logger.LogCritical("system is not catching this error {errorMessage} from {endPoint}", exception.Message, endpoint);
         
        return ps.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Type = exception.GetType().Name,
                    Title = "Exception occured",
                    Detail = exception.Message
                }
            }
        );
    }
}