using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Zhoubin.Infrastructure.Common.Web
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AuthorizeMiddleware
    {
        private readonly Func<string, List<string>> _queryUserRoles;
        public AuthorizeMiddleware(Func<string, List<string>> queryUserRoles)
        {
            _queryUserRoles = queryUserRoles;
        }
        private readonly RequestDelegate _next;

        public AuthorizeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var identity = httpContext.User.Identity;
            if (identity.IsAuthenticated)
            {
                var roles = _queryUserRoles(identity.Name);
                foreach (var role in roles)
                    httpContext.User.AddIdentity(new ClaimsIdentity("Basic", ClaimTypes.Role, role));
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
