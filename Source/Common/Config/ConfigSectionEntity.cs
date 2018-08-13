using System.Collections.Generic;

namespace Zhoubin.Infrastructure.Common.Config
{
    /// <summary>
    /// 配置区读取辅助类
    /// </summary>
    /// <typeparam name="T">要求<see cref="ConfigEntityBase"/>的子类</typeparam>
    public class ConfigSectionEntity<T> where T : ConfigEntityBase
    {
        /// <summary>
        /// 读取配置实体列表
        /// </summary>
        public List<T> Enities { get; internal set; }

        /// <summary>
        /// 默认配置
        /// </summary>
        public T DefaultConfig { get; internal set; }

        /// <summary>
        /// 默认配置名称
        /// </summary>
        internal string DefaultConfigName { get; set; }
    }
}
