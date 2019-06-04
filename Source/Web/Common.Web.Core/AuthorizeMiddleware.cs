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
        private readonly Func<IIdentity, IIdentity> _changeIdentity;

        public AuthorizeMiddleware(RequestDelegate next, Func<string, List<string>> queryUserRoles, Func<IIdentity, IIdentity> changeIdentity)
        {
            _next = next;
            _queryUserRoles = queryUserRoles;
            _changeIdentity = changeIdentity;
        }

        public Task Invoke(HttpContext httpContext)
        {
            if (httpContext.User != null && httpContext.User.Identity != null)
            {
                var identity = httpContext.User.Identity;
                if (identity.IsAuthenticated)
                {
                    var id1 = _changeIdentity(identity);
                    if (_queryUserRoles != null)
                    {
                        ClaimsIdentity ci = id1 as ClaimsIdentity;
                        if (ci != null)
                        {
                            foreach (var c in ci.Claims.Where(p => p.Type == ClaimsIdentity.DefaultRoleClaimType).ToList())
                            {
                                ci.RemoveClaim(c);
                            }
                        }
                        var roles = _queryUserRoles(identity.Name);
                        httpContext.User = new GenericPrincipal(id1, roles.ToArray());
                    }
                    else
                    {
                        httpContext.User = new GenericPrincipal(id1, new string[0]);
                    }
                }
            }
            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthorizeMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthorizeMiddleware(this IApplicationBuilder builder, Func<IIdentity, IIdentity> changeIdentity, Func<string, List<string>> queryUserRoles = null)
        {
            return builder.UseMiddleware<AuthorizeMiddleware>(queryUserRoles, changeIdentity);
        }
        public static IApplicationBuilder UseAuthorizeMiddleware(this IApplicationBuilder builder, Func<string, List<string>> queryUserRoles)
        {
            Func<IIdentity, IIdentity> changeIdentity = identity => identity;
            return UseAuthorizeMiddleware(builder, changeIdentity, queryUserRoles);
        }
    }
}
