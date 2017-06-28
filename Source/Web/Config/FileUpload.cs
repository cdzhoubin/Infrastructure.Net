using System.Collections.Generic;
using System.Xml;
using Zhoubin.Infrastructure.Common.Config;

namespace Zhoubin.Infrastructure.Web.Config
{
    /// <summary>
    /// 文件上传配置
    /// </summary>
    public class FileUploadHelper : ConfigHelper<FileUploadEntity>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public FileUploadHelper()
            : base("FileUpload", null)
        {

        }

        /// <summary>
        /// 上传配置
        /// </summary>
        public FileUploadEntity UploadConfig
        {
            get { return Section[0]; }
        }
    }

    /// <summary>
    /// 文件上传配置处理器
    /// </summary>
    public class FileUploadSection : ConfigurationSectionHandlerHelper<FileUploadEntity>
    {

    }
    /// <summary>
    ///文件上传配置Entity
    /// </summary>
    public class FileUploadEntity : ConfigEntityBase<FileUploadEntity>
    {
        /// <summary>
        /// 公共设置
        /// </summary>
        public UploadCommonEntity CommonSet { set; get; }
        /// <summary>
        /// 设置列表
        /// </summary>
        public List<UploadEntity> UploadList { get; set; }


        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="node">结点</param>
        protected override void SetProperty(FileUploadEntity entity, XmlNode node)
        {

            switch (node.Name.ToLower())
            {
                case "name":
                    entity.Name = node.InnerText;
                    break;
                case "commonset":
                    entity.CommonSet = new UploadCommonEntity().Parse(node);
                    break;
                case "uploadlist":
                    entity.UploadList = new List<UploadEntity>();
                    {
                        foreach (XmlNode child1 in node.ChildNodes)
                        {
                            entity.UploadList.Add(UploadEntity.CreateUploadEntity(entity.CommonSet).Parse(child1));
                        }
                    }
                    break;

            }

        }
    }

    /// <summary>
    /// 上传公共配置实体
    /// </summary>
    public class UploadCommonEntity : CommonSetBase<UploadCommonEntity>
    {
        /// <summary>
        /// 属性设置
        /// </summary>
        /// <param name="entity">待设置对象</param>
        /// <param name="node">节点内容</param>
        protected override void SetProperty(UploadCommonEntity entity, XmlNode node)
        {

            if (string.IsNullOrEmpty(node.InnerText))
            {
                return;
            }

            SetCommonProperty(entity, node);

        }
    }

    /// <summary>
    /// 公共配置基类
    /// </summary>
    /// <typeparam name="T"><see cref="ConfigEntityBase"/>的子类类型</typeparam>
    public abstract class CommonSetBase<T> : ConfigEntityBase<T> where T : ConfigEntityBase, new()
    {
        /// <summary>
        /// 文件上传处理器路径，可以是一个ashx、aspx文件或web服务
        /// </summary>
        public string HandlerPath { get; set; }

        /// <summary>
        /// 文件上传组件目录
        /// </summary>
        public string PlUploadFloder { get; set; }

        /// <summary>
        /// 文件类型列表
        /// </summary>
        public string FileTypeList { get; set; }

        /// <summary>
        /// 最大文件限制,mb
        /// </summary>
        public string MaxFileSize { get; set; }

        /// <summary>
        /// 传输块大小，mb
        /// </summary>
        public string ChunkSize { get; set; }

        /// <summary>
        /// 运行时支持
        /// </summary>
        public string RunTypes { get; set; }

        /// <summary>
        /// 上传时文件夹编号
        /// </summary>
        public string FolderId { get; set; }

        /// <summary>
        /// 单个文件上传模板
        /// </summary>
        public string SingleTemplate { get; set; }

        /// <summary>
        /// 多文件上传模板
        /// </summary>
        public string MoreTemplate { get; set; }

        /// <summary>
        /// 设置属性值 
        /// </summary>
        /// <param name="entity">对象</param>
        /// <param name="node">节点</param>
        /// <typeparam name="T1">对象类型</typeparam>
        protected void SetCommonProperty<T1>(T1 entity, XmlNode node) where T1 : CommonSetBase<T>
        {
            switch (node.Name.ToLower())
            {
                case "name":
                    entity.Name = node.InnerText;
                    break;
                case "handlerpath":
                    entity.HandlerPath = node.InnerText;
                    break;
                case "pluploadfloder":
                    entity.PlUploadFloder = node.InnerText;
                    break;
                case "filetypelist":
                    entity.FileTypeList = node.InnerText;
                    break;
                case "runtypes":
                    entity.RunTypes = node.InnerText;
                    break;
                case "maxfilesize":
                    entity.MaxFileSize = node.InnerText;
                    break;
                case "chunksize":
                    entity.ChunkSize = node.InnerText;
                    break;
                case "folderid":
                    entity.FolderId = node.InnerText;
                    break;
                case "singletemplate":
                    entity.SingleTemplate = node.InnerText;
                    break;
                case "moretemplate":
                    entity.MoreTemplate = node.InnerText;
                    break;

            }
        }
    }

    /// <summary>
    /// 上传文档实体
    /// </summary>
    public class UploadEntity : CommonSetBase<UploadEntity>
    {
        /// <summary>
        /// 根据公共配置创建上传配置
        /// </summary>
        /// <param name="entity">公共配置</param>
        /// <returns>返回上传配置</returns>
        internal static UploadEntity CreateUploadEntity(UploadCommonEntity entity)
        {
            return new UploadEntity
                              {
                                  ChunkSize = entity.ChunkSize,
                                  FileTypeList = entity.FileTypeList,
                                  HandlerPath = entity.HandlerPath,
                                  MaxFileSize = entity.MaxFileSize,
                                  PlUploadFloder = entity.PlUploadFloder,
                                  RunTypes = entity.RunTypes,
                                  FolderId = entity.FolderId,
                                  SingleTemplate = entity.SingleTemplate,
                                  MoreTemplate = entity.MoreTemplate,
                              };
        }
        /// <summary>
        /// 图片文件最大宽度
        /// </summary>
        public int ImageWidth { get; set; }
        /// <summary>
        /// 图片文件最大高度
        /// </summary>
        public int ImageHeight { get; set; }

        /// <summary>
        /// 是否图片文件上传
        /// </summary>
        public bool ImageUpload { get; set; }

        /// <summary>
        /// 解析节点到实体UploadEntity
        /// </summary>
        /// <param name="node">待解析的xml配置节点</param>
        /// <returns>解析后的实体UploadEntity</returns>
        public override UploadEntity Parse(XmlNode node)
        {
            return Parse(this, node);
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        /// <param name="entity">上传配置对象</param>
        /// <param name="node">配置对象</param>
        protected override void SetProperty(UploadEntity entity, XmlNode node)
        {
            if (string.IsNullOrEmpty(node.InnerText))
            {
                return;
            }

            switch (node.Name.ToLower())
            {
                case "imageupload":
                    entity.ImageUpload = node.InnerText.ToLower() == "true";
                    break;
                case "imagewidth":
                    entity.ImageWidth = GetIntValue(node.InnerText, 1000);
                    break;
                case "imageheight":
                    entity.ImageHeight = GetIntValue(node.InnerText, 1000);
                    break;
                default:
                    SetCommonProperty(entity, node);
                    break;
            }

        }

        private int GetIntValue(string str, int defaultValue)
        {
            int tmp;
            if (int.TryParse(str, out tmp) && tmp > 0)
            {
                return tmp;
            }

            return defaultValue;
        }
    }

}
