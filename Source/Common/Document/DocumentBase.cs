using System;
using System.Data;
using System.IO;

namespace Zhoubin.Infrastructure.Common.Document
{
    /// <summary>
    /// 文档对象基类
    /// </summary>
    public abstract class DocumentBase : IDocument
    {
        private readonly bool _readMode;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="readMode">读写模式</param>
        protected DocumentBase(bool readMode)
        {
            _readMode = readMode;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            Initializing();
            DocumentInitialize();
            Initialized();
        }


        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void DocumentInitialize();
        /// <summary>
        /// 初始化开始
        /// </summary>
        protected virtual void Initializing()
        {
        }

        /// <summary>
        /// 初始化完成
        /// </summary>
        protected virtual void Initialized()
        {
        }

        /// <summary>
        /// 打开
        /// </summary>
        public void Open()
        {
            Openging();
            DocumentOpen();
            Opened();
        }


        /// <summary>
        /// 执行打开文档
        /// </summary>
        protected abstract void DocumentOpen();
        /// <summary>
        /// 正在打开文档
        /// </summary>
        protected virtual void Openging()
        {
        }

        /// <summary>
        /// 打开文档完成
        /// </summary>
        protected virtual void Opened()
        {
        }


        /// <summary>
        /// 保存对象
        /// </summary>
        /// <param name="stream">保存到目标的文件流</param>
        public void Save(Stream stream)
        {
            if (_readMode)
            {
                throw new ReadOnlyException("当前处理读取模式，不允许保存文档。");
            }

            DocumentSaving();
            DocumentSave(stream);
            DocumentSaved();
        }

        /// <summary>
        /// 保存文档
        /// </summary>
        /// <param name="stream">目标流</param>
        protected abstract void DocumentSave(Stream stream);
        /// <summary>
        /// 正在保存文档
        /// </summary>
        protected virtual void DocumentSaving()
        {
        }

        /// <summary>
        /// 文档保存完成
        /// </summary>
        protected virtual void DocumentSaved()
        {
        }
        
        /// <summary>
        /// 自动释放对象
        /// </summary>
        public virtual void Dispose()
        {
        }


        /// <summary>
        /// 读取文档
        /// </summary>
        public void Read()
        {
            if (!_readMode)
            {
                throw new Exception("当前处理写入模式，不允许读取文档。");
            }

            DocumentReading();
            DocumentRead();
            DocumentReaded();
        }

        /// <summary>
        /// 文档读取
        /// </summary>
        protected virtual void DocumentRead()
        {
            
        }

        /// <summary>
        /// 正在保存文档
        /// </summary>
        protected virtual void DocumentReading()
        {
        }

        /// <summary>
        /// 文档保存完成
        /// </summary>
        protected virtual void DocumentReaded()
        {
        }
    }
}
