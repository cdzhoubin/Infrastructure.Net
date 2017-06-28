using System.Collections.Generic;
using System.Xml;
using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.Web.Validate
{
    /// <summary>
    /// 数据检验配置实体
    /// </summary>
    public sealed class ValidateEntity : ConfigEntityBase<ValidateEntity>
    {
        /// <summary>
        /// 正则表达式
        /// </summary>
        public string RegularExpression { get; private set; }
        /// <summary>
        /// 出错时显示的信息
        /// </summary>
        public string ErrorMessage { get; private set; }
        /// <summary>
        /// 未定义的扩展属性
        /// </summary>
        public Dictionary<string, string> ExtentProperty { get; set; }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="node">结点</param>
        protected override void SetProperty(ValidateEntity entity, XmlNode node)
        {
            switch (node.Name)
                {
                    case "RegularExpression":
                        entity.RegularExpression = node.InnerText;
                        break;
                    case "ErrorMessage":
                        entity.ErrorMessage = node.InnerText;
                        break;
                    default:
                        if (string.IsNullOrEmpty(node.Name))
                        {
                            entity.ExtentProperty.Add(node.Name, node.InnerText);
                        }
                        break;
                }
            }
        }
    
}