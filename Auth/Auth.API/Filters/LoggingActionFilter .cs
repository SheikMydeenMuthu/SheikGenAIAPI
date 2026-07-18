using Microsoft.AspNetCore.Mvc.Filters;

namespace Auth.API.Filters;

public class LoggingActionFilter : IActionFilter
{
    private readonly ILogger<LoggingActionFilter> _logger;
    private static readonly string[] SensitiveKeys = { "password", "token", "refreshtoken", "secret" };

    public LoggingActionFilter(ILogger<LoggingActionFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var safeArgs = context.ActionArguments.ToDictionary(
            kvp => kvp.Key,
            kvp => (object?)RedactIfSensitive(kvp.Key, kvp.Value));

        _logger.LogInformation("Executing {Action} with args: {@Args}",
            context.ActionDescriptor.DisplayName, safeArgs);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("Executed {Action}, StatusCode: {StatusCode}",
            context.ActionDescriptor.DisplayName,
            context.HttpContext.Response.StatusCode);
    }

    private static object? RedactIfSensitive(string key, object? value)
    {
        if (SensitiveKeys.Any(k => key.Contains(k, StringComparison.OrdinalIgnoreCase)))
            return "***REDACTED***";

        // Also redact known DTO properties like LoginCommand.Password
        if (value != null)
        {
            var props = value.GetType().GetProperties();
            var hasSensitiveProp = props.Any(p => SensitiveKeys.Any(k => p.Name.Contains(k, StringComparison.OrdinalIgnoreCase)));
            if (hasSensitiveProp)
            {
                var dict = props.ToDictionary(
                    p => p.Name,
                    p => SensitiveKeys.Any(k => p.Name.Contains(k, StringComparison.OrdinalIgnoreCase))
                        ? "***REDACTED***"
                        : p.GetValue(value)?.ToString());
                return dict;
            }
        }

        return value;
    }
}