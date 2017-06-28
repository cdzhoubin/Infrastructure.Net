using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;

namespace Zhoubin.Infrastructure.Common.Config
{
    /// <summary>
    /// 配置读取类基类
    /// </summary>
    /// <typeparam name="T">配置对象类型</typeparam>
    public class ConfigHelper<T> where T : ConfigEntityBase
    {
        private readonly ConfigSectionEntity<T> _section;
        /// <summary>
        /// 默认配置
        /// </summary>
        public T DefaultConfig { get { return _section.DefaultConfig; } }

        /// <summary>
        /// 解析后的配置
        /// </summary>
        protected List<T> Section { get { return _section.Enities; } }

        /// <summary>
        /// 加载配置信息
        /// </summary>
        /// <param name="information">配置信息</param>
        /// <returns>返回加载成功的配置信息</returns>
        protected ConfigSectionEntity<T> LoadSection(SectionInformation information)
        {
            return LoadSection<T>(information);
        }

        private ConfigSectionEntity<T1> LoadSection<T1>(SectionInformation information) where T1 : ConfigEntityBase
        {
            var strs = information.Type.Split(",".ToArray(), 2);
            var handler = (IConfigurationSectionHandler)Assembly.Load(strs[1]).CreateInstance(strs[0]);
            var doc = new XmlDocument();
            doc.LoadXml(information.GetRawXml());
            if (handler != null)
            {
                return (ConfigSectionEntity<T1>)handler.Create(null, null, doc.ChildNodes[0]);
            }

            return null;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sectionName">配置区名称</param>
        /// <param name="configFile">配置文件路径，当传入null或空时，如果在当前目录或bin目录下存在Common.Config文件，就使用此文件，如果不存在表示从默认配置文件读取</param>
        public ConfigHelper(string sectionName, string configFile)
        {
            if (string.IsNullOrEmpty(configFile))
            {
                configFile = GetConfigFile("Common.Config");
                if (!File.Exists(configFile))
                {
                    configFile = GetConfigFile("bin\\Common.Config");
                    if (!File.Exists(configFile))
                    {
                        _section = (ConfigSectionEntity<T>)ConfigurationManager.GetSection(sectionName);
                        return;
                    }
                }
            }

            var configMap = new ExeConfigurationFileMap { ExeConfigFilename = configFile };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
            var configurationSection = config.GetSection(sectionName);
            if (configurationSection != null)
            {
                _section = LoadSection(configurationSection.SectionInformation);
            }
        }

        private string GetConfigFile(string file)
        {
            return HttpContext.Current != null ? HttpContext.Current.Server.MapPath("~\\" + file) : ".\\" + file;
        }

        /// <summary>
        /// 根据索引键值取指定的配置
        /// </summary>
        /// <param name="name"></param>
        public T this[string name]
        {
            get { return Section.FirstOrDefault(p => p.Name == name); }
        }

        /// <summary>
        /// 根据索引键值取指定的配置
        /// </summary>
        /// <param name="index"></param>
        public T this[int index]
        {
            get
            {
                if (index <0 || index >Section.Count-1)
                {
                    return null;
                }

                return Section[index];
            }
        }
        /// <summary>
        /// 配置项个数
        /// </summary>
        public int Count
        {
            get { return Section.Count; }
        }
    }

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
