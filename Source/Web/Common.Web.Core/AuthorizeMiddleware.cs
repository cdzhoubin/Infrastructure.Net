using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Zhoubin.Infrastructure.Common.Web
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AuthorizeMiddleware
    {
        private readonly Func<string, List<string>> _queryUserRoles;
        private readonly RequestDelegate _next;

        public AuthorizeMiddleware(RequestDelegate next, Func<string, List<string>> queryUserRoles)
        {
            _next = next; _queryUserRoles = queryUserRoles;
        }

        public Task Invoke(HttpContext httpContext)
        {
            if (httpContext.User != null && httpContext.User.Identity != null)
            {
                var identity = httpContext.User.Identity;
                if (identity.IsAuthenticated)
                {
                    ClaimsIdentity ci = identity as ClaimsIdentity;
                    if (ci != null)
                    {
                        foreach (var c in ci.Claims.Where(p => p.Type == ClaimsIdentity.DefaultRoleClaimType).ToList())
                        {
                            ci.RemoveClaim(c);
                        }
                    }
                    var roles = _queryUserRoles(identity.Name);
                    httpContext.User = new GenericPrincipal(ci, roles.ToArray());
                }
            }
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthorizeMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthorizeMiddleware(this IApplicationBuilder builder, Func<string, List<string>> queryUserRoles)
        {
            return builder.UseMiddleware<AuthorizeMiddleware>(queryUserRoles);
        }
    }
}
