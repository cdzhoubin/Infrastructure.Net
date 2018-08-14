using System.Collections.Generic;

namespace Zhoubin.Infrastructure.Common.Web.Permission
{
    /// <summary>
    /// 授权检查接口
    /// </summary>
    public interface IPerssionsAuthorization
    {
        /// <summary>
        /// 授权检查
        /// </summary>
        /// <param name="userPermission">待检查权限</param>
        /// <returns>授权成功返回true,其他返回false</returns>
        bool Authorization(UserPermission userPermission);
    }
}
