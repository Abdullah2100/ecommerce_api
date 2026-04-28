using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api.Exceptions;

public  class GlobalExceptionHandler(IProblemDetailsService ps) :IExceptionHandler
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

        return ps.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Type = exception.GetType().Name,
                    Title ="Exception occured",
                    Detail = exception.Message
                }
            }
        );
    }
}