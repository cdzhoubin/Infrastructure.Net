namespace Zhoubin.Infrastructure.Common.Ioc
{
    /// <summary>
    /// Ioc�����ӿ�
    /// </summary>
    public interface IResolver
    {
        /// <summary>
        /// Ioc��������ע��ӿ�
        /// </summary>
        /// <typeparam name="T">��������</typeparam>
        /// <param name="name">��������</param>
        /// <returns>�����ɹ�����</returns>
        T Resolve<T>(string name=null);
    }
}