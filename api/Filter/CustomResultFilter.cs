using api.application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api.Filter;

public class CustomResultFilter() : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult { Value: ObjectResult value } result)
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