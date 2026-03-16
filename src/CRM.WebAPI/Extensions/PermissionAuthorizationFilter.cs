using System.Security.Claims;
using CRM.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRM.WebAPI.Extensions;

public sealed class PermissionAuthorizationFilter(
    IPermissionRepository permissionRepository,
    string resource,
    string action
) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var userIdClaim = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var hasPermission = await permissionRepository.UserHasPermissionAsync(userId, resource, action);
        if (!hasPermission)
        {
            context.Result = new ObjectResult(new ProblemDetails
            {
                Title = "Forbidden",
                Detail = $"No tiene permiso para '{resource}.{action}'.",
                Status = StatusCodes.Status403Forbidden
            })
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
