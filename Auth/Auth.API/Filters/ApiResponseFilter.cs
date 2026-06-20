using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auth.API.Filters;

public class ApiResponseFilter : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ObjectResult objectResult)
        {
            var wrappedResponse = new
            {
                success = true,
                data = objectResult.Value,
                timestamp = DateTime.UtcNow
            };

            objectResult.Value = wrappedResponse;
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        // runs after result is sent to client (e.g. for logging response time)
    }
}