using api.application.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api.Filter;

public  class CustomResultFilter() : IAsyncActionFilter
{
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var result = context.Result as ObjectResult;
        var resultJson = new Result
        (
            result?.StatusCode is >= 200 or 400,
            result?.StatusCode is >= 400 or 500 ? result.Value?.ToString() : null,
            result?.Value,
            (int)result?.StatusCode!
        );
        result.Value = System.Text.Json.JsonSerializer.Serialize(resultJson);
        return next();
    }
}