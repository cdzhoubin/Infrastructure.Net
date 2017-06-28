using System;
using System.IO;

namespace Zhoubin.Infrastructure.Common.Document
{
    /// <summary>
    /// 文档接口
    /// </summary>
    public interface IDocument : IDisposable
    {
        /// <summary>
        /// 打开
        /// </summary>
        void Open();
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="stream">保存到目标的文件流</param>
        void Save(Stream stream);
        /// <summary>
        /// 初始化
        /// </summary>
        void Initialize();

        /// <summary>
        /// 文档读取
        /// </summary>
        void Read();
    }
}