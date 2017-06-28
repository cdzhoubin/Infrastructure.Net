namespace Zhoubin.Infrastructure.Web.Validate
{
    /// <summary>
    /// NetValidate使用的校验类型
    /// </summary>
    public enum NetValidateType
    {
        /// <summary>
        /// 邮件
        /// </summary>
        Email,
        /// <summary>
        /// 电话
        /// </summary>
        Phone,
        /// <summary>
        /// 移动电话
        /// </summary>
        Mobile,
        /// <summary>
        /// http地址
        /// </summary>
        Url,
    }
}