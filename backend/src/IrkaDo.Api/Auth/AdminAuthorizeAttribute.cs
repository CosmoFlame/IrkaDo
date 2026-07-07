using IrkaDo.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace IrkaDo.Api.Auth;

/// <summary>
/// Gates admin endpoints behind the signed session token issued by the login endpoint.
/// Reads <c>Authorization: Bearer &lt;token&gt;</c> and validates it via <see cref="IAdminTokenService"/>.
/// Deliberately lightweight (no full auth scheme) to match the single-password model.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AdminAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var tokenService = context.HttpContext.RequestServices.GetRequiredService<IAdminTokenService>();

        var header = context.HttpContext.Request.Headers[HeaderNames.Authorization].ToString();
        const string scheme = "Bearer ";

        var token = header.StartsWith(scheme, StringComparison.OrdinalIgnoreCase)
            ? header[scheme.Length..].Trim()
            : null;

        if (token is null || !tokenService.TryValidate(token))
        {
            context.Result = new UnauthorizedResult();
        }

        return Task.CompletedTask;
    }
}
