using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RankingPadelAPI.Domain;
using RankingPadelAPI.Services;

namespace RankingPadelAPI.Filters;

public sealed class AuthorizationFilterAttribute : AuthenticationFilterAttribute
{
    private readonly string? _permission;

    public AuthorizationFilterAttribute(string? permission = null)
    {
        _permission = permission;
    }

    public override void OnAuthorization(AuthorizationFilterContext context)
    {
        base.OnAuthorization(context);

        if (context.Result != null)
        {
            return;
        }

        var userLogged = context.HttpContext.Items[Items.UserLogged];

        if (userLogged == null)
        {
            context.Result = new ObjectResult(new
            {
                InnerCode = "Unauthenticated",
                Message = "You are not authenticated"
            })
            { StatusCode = (int)HttpStatusCode.Unauthorized };
            return;
        }

        var userLoggedMapped = (User)userLogged;

        var permission = BuildPermission(context);

        var hasNotPermission = !userLoggedMapped.Role.HasPermission(permission);

        if (hasNotPermission)
        {
            context.Result = new ObjectResult(new
            {
                InnerCode = "Forbidden",
                Message = $"Missing permission {permission}"
            })
            { StatusCode = (int)HttpStatusCode.Forbidden };
        }
    }

    public Permission BuildPermission(AuthorizationFilterContext context)
    {
        return new Permission(_permission ?? $"{context.RouteData.Values["action"].ToString().ToLower()}-{context.RouteData.Values["controller"].ToString().ToLower()}");

    }
}
