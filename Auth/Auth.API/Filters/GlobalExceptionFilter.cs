using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auth.API.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ValidationException validationEx)
        {
            context.Result = new BadRequestObjectResult(new
            {
                errors = validationEx.Errors.Select(e => e.ErrorMessage)
            });
            context.ExceptionHandled = true;
        }
        else if (context.Exception is UnauthorizedAccessException)
        {
            context.Result = new UnauthorizedObjectResult(new
            {
                error = context.Exception.Message
            });
            context.ExceptionHandled = true;
        }
        else if (context.Exception is InvalidOperationException)
        {
            context.Result = new ConflictObjectResult(new
            {
                error = context.Exception.Message
            });
            context.ExceptionHandled = true;
        }
        else
        {
            context.Result = new ObjectResult(new
            {
                error = "An unexpected error occurred."
            })
            {
                StatusCode = 500
            };
            context.ExceptionHandled = true;
        }
    }
}