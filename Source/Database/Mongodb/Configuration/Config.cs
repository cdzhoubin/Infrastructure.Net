using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.Common.MongoDb.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class Config: ConfigHelper<ConnectionEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        public Config() : base("MongoConfig")
        {
        }
    }
}