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
        /// 初始化
        /// </summary>
        void Initialize();

        
    }
    /// <summary>
    /// 文档写入接口
    /// </summary>
    public interface IDocumentWriter :IDocument
    {
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="stream">保存到目标的文件流</param>
        void Save(Stream stream);
    }
    /// <summary>
    /// 文档读取接口
    /// </summary>
    /// <typeparam name="T">读取返回的数据类型</typeparam>
    public interface IDocumentReader<T>: IDocument
{
        /// <summary>
        /// 文档读取
        /// </summary>
        /// <returns>返回读取到的数据</returns>
        T Read();
    }
}