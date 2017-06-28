using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.Web.UserControlToHtml
{
    /// <summary>
    /// 控件生成Html代码配置实体
    /// </summary>
    public class HtmlEntity : ConfigEntityBase<HtmlEntity>
    {
        /// <summary>
        /// 控件路径，以~相同路径开头
        /// </summary>
        public string Url { get; private set; }
        /// <summary>
        /// 安全信息配置
        /// </summary>
        public List<string> SecurityIdentification { get; private set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public HtmlEntity()
        {
            SecurityIdentification = new List<string>();
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="node">结点</param>
        protected override void SetProperty(HtmlEntity entity, XmlNode node)
        {
            switch (node.Name)
            {
                case "Url":
                    entity.Url = node.InnerText;
                    break;
                case "SecurityIdentification":
                    {
                        if (!string.IsNullOrEmpty(node.InnerText))
                        {
                            entity.SecurityIdentification.AddRange(node.InnerText.Split(',').Where(str => !string.IsNullOrEmpty(str)));
                        }
                    }
                    break;
            }
        }
    }
}