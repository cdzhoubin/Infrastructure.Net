using System.Linq;
using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.Log.Config
{   
    /// <summary>
    /// 日志配置实体
    /// </summary>
    public sealed class LogConfigEntity:ConfigEntityBase<LogConfigEntity>
    {
        /// <summary>
        /// 日志处理器
        /// </summary>
        public string HandleType { get; set; }

        /// <summary>
        /// 默认处理配置
        /// </summary>
        public bool  Default { get; set; }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="node">结点</param>
        protected override void SetProperty(LogConfigEntity entity, System.Xml.XmlNode node)
        {
            switch (node.Name)
            {
                case "HandleType":
                    entity.HandleType = node.InnerText;
                    break;
                case "Default":
                    entity.Default = node.InnerText == "true";
                    break;
            }
        }
    }

    
}
