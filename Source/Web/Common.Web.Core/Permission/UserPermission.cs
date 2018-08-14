using Microsoft.AspNetCore.Builder;
using System;
using System.Text;

namespace Zhoubin.Infrastructure.Common.Web.Permission
{
    /// <summary>
    /// 用户权限定义
    /// </summary>
    public sealed class UserPermission
    {
        public UserPermission(string id, string url)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        /// <summary>
        /// 用户标识（Id或用户，唯一识别用户）
        /// </summary>
        public string Id
        {
            get;
            private set;
        }
        /// <summary>
        /// 资源
        /// </summary>
        public string Url
        {
            get;
            private  set;
        }
    }

    public static class PermissionMiddlewareExtensions
    {
        /// <summary>
        /// 引入权限中间件
        /// </summary>
        /// <param name="builder">扩展类型</param>
        /// <param name="option">权限中间件配置选项</param>
        /// <returns></returns>
        public static IApplicationBuilder UsePermission(
              this IApplicationBuilder builder, PermissionOption option)
        {
            return builder.UseMiddleware<PermissionUserUrlMiddleware>(option);
        }
    }
}
