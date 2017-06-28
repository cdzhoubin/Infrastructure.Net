namespace Zhoubin.Infrastructure.Common.Ioc
{
    /// <summary>
    /// Ioc容器接口
    /// </summary>
    public interface IResolver
    {
        /// <summary>
        /// Ioc创建对象注入接口
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">配置名称</param>
        /// <returns>创建成功类型</returns>
        T Resolve<T>(string name=null);
    }
}