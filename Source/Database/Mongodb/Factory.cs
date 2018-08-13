using Zhoubin.Infrastructure.Common.Extent;

namespace Zhoubin.Infrastructure.Common.MongoDb
{
    /// <summary>
    /// 工厂类
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// 通过反射生成文件存储对象
        /// </summary>
        /// <param name="name">Config节点下面的Name节点</param>
        /// <returns>实现了IFileStorage存储的类型</returns>
        public static IFileStorage CreateFileStorage(string name)
        {
            return CreateStorage<IFileStorage>(name);
        }

        /// <summary>
        /// 通过反射生成对象存储对象
        /// </summary>
        /// <param name="name">Config节点下面的Name节点</param>
        /// <returns>实现了IObjectStorage存储的类型</returns>
        public static IObjectStorage CreateObjectStorage(string name)
        {
            return CreateStorage<IObjectStorage>(name);
        }

        private static T CreateStorage<T>(string name)
        {
            var config = new Config();
            var entity = config[name];
            return entity.Type.CreateInstance<T>(entity.Name);
        }

        /// <summary>
        /// 通过反射生成文件存储对象
        /// </summary>
        /// <returns>文件存储对象</returns>
        public static IFileStorage CreateFileStorage()
        {
            return CreateStorage<IFileStorage>("File");
        }

        /// <summary>
        /// 通过反射生成对象存储对象
        /// </summary>
        /// <returns>对象存储对象</returns>
        public static IObjectStorage CreateObjectStorage()
        {
            return CreateStorage<IObjectStorage>("Object");
        }
    }
}
