using System.Xml;
using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.Common.MongoDb
{
    /// <summary>
    /// MongoDB配置文件类
    /// </summary>
    public class Config : ConfigHelper<ConnectionEntity>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Config() : base("MongoConfig") { }

        ///// <summary>
        ///// 通过索引获取指定的配置数据
        ///// </summary>
        ///// <param name="index"></param>
        //public ConnectionEntity this[int index]
        //{
        //    get { return Section[index]; }
        //}
    }

    /// <summary>
    /// MongoDB配置实体类
    /// </summary>
    public class ConnectionEntity : ConfigEntityBase
    {

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString
        {
            get { return GetValue<string>("ConnectionString"); }
            set { SetValue("ConnectionString", value); }
        }
        /// <summary>
        /// 类型：Provider类型名称
        /// </summary>
        public string Type
        {
            get { return GetValue<string>("Type"); }
            set { SetValue("Type", value); }
        }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DataBase
        {
            get { return GetValue<string>("DataBase"); }
            set { SetValue("DataBase", value); }
        }



        /// <summary>
        /// 对关键数据进行加密进，重载此方法进行解密
        /// </summary>
        /// <param name="entity">待解密对象</param>
        /// <returns>返回解密后对象</returns>
        protected override void Decrypt()
        {
            base.Decrypt();
            ConnectionString = Decrypt(ConnectionString);
        }
    }
}
