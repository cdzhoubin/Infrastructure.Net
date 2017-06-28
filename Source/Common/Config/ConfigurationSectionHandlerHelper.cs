using System.Configuration;
using System.Linq;
using System.Xml;

namespace Zhoubin.Infrastructure.Common.Config
{
    /// <summary>
    /// 配置读取Provider
    /// 基于.net架构读取方法
    /// </summary>
    /// <typeparam name="T">此类型要求是<see cref="ConfigEntityBase"/>的子类</typeparam>
    public abstract class ConfigurationSectionHandlerHelper<T> : IConfigurationSectionHandler
        where T : ConfigEntityBase<T>, new()
    {
        /// <summary>
        /// 解析配置信息
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="configContext">配置上下文</param>
        /// <param name="section">配置节点内容</param>
        /// <returns>返回解析后的对象</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            var sectionEntity = new ConfigSectionEntity<T>
            {
                Enities = (from XmlNode node in section.ChildNodes select new T().Parse(node)).ToList()
            };

            if (section.Attributes == null || section.Attributes["Default"] == null)
            {
                return sectionEntity;
            }

            sectionEntity.DefaultConfigName = section.Attributes["Default"].Value;
            if (sectionEntity.Enities.Any(p => p.Name == sectionEntity.DefaultConfigName))
            {
                sectionEntity.DefaultConfig =
                    sectionEntity.Enities.First(p => p.Name == sectionEntity.DefaultConfigName);
            }
            else
            {
                throw new ConfigurationErrorsException("默认定义节点错误。",section);
            }

            return sectionEntity;
        }
    }
}
