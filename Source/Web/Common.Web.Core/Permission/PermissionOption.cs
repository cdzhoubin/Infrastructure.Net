namespace Zhoubin.Infrastructure.Common.Web.Permission
{
    public class PermissionOption
    {
        /// <summary>
        /// 登录成功action
        /// </summary>
        public string LoginAction
        { get; set; }
        /// <summary>
        /// 无权限action
        /// </summary>
        public string NoPermissionAction
        { get; set; }

        /// <summary>
        /// 用户权限集合
        /// </summary>
        public IPerssionsAuthorization UserPerssions { get; set; }
    }
}
