using api.application.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api.Filter;

public class CustomResultFilter() : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var result = context.Result as ObjectResult;

        if (result?.Value is ObjectResult value)
        {
            var resultJson = new Result
            (
                value?.StatusCode is >= 200 or < 400,
                value?.StatusCode is >= 400 or <= 500 ? value?.ToString() : null,
                value?.Value,
                (int)result.StatusCode!
            );
            value?.Value = resultJson;
        }

        await next();
    }
}