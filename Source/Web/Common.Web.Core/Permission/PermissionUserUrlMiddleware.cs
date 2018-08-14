using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Zhoubin.Infrastructure.Common.Web.Permission
{
    /// <summary>
    /// 用户资源权限中间件
    /// </summary>
    public sealed class PermissionUserUrlMiddleware
    {
        /// <summary>
        /// 管道代理对象
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// 权限中间件的配置选项
        /// </summary>
        private readonly PermissionOption _option;

        /// <summary>
        /// 用户权限集合
        /// </summary>
        internal static IPerssionsAuthorization _userPermissions;

        /// <summary>
        /// 权限中间件构造
        /// </summary>
        /// <param name="next">管道代理对象</param>
        /// <param name="permissionResitory">权限仓储对象</param>
        /// <param name="option">权限中间件配置选项</param>
        public PermissionUserUrlMiddleware(RequestDelegate next, PermissionOption option)
        {
            _option = option;
            _next = next;
            _userPermissions = option.UserPerssions;
        }
        /// <summary>
        /// 调用管道
        /// </summary>
        /// <param name="context">请求上下文</param>
        /// <returns></returns>
        public Task Invoke(HttpContext context)
        {
            //请求Url
            var questUrl = context.Request.Path.Value.ToLower();

            //是否经过验证
            var isAuthenticated = context.User.Identity.IsAuthenticated;
            if (isAuthenticated)
            {
                var userName = context.User.Claims.SingleOrDefault(s => s.Type == ClaimTypes.Sid);

                if (userName != null && !string.IsNullOrEmpty(userName.Value)
                    && _userPermissions.Authorization(new UserPermission(userName.Value, questUrl)))
                {
                    return this._next(context);
                }
            }
            //无权限跳转到拒绝页面
            context.Response.Redirect(_option.NoPermissionAction);
            return this._next(context);
        }
    }
}
