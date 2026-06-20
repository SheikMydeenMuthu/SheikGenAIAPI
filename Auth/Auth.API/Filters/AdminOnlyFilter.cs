using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auth.API.Filters;

public class AdminOnlyFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
         .Any(em => em is Microsoft.AspNetCore.Authorization.IAllowAnonymous);

        if (hasAllowAnonymous)
            return;
    
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Not authenticated" });
            return;
        }

        var role = user.FindFirst("role")?.Value;

        if (role != "Admin")
        {
            context.Result = new ObjectResult(new { error = "Admin access required" })
            {
                StatusCode = 403
            };
        }
    }
}