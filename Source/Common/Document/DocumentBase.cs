using System;
using System.Data;
using System.IO;

namespace Zhoubin.Infrastructure.Common.Document
{
    public delegate void DocumentHandler(object sender, object e);
    /// <summary>
    /// 文档对象基类
    /// </summary>
    public abstract class DocumentBase<T> : IDocumentWriter,IDocumentReader<T>
    {
        private readonly bool _readMode;
        private bool _isInitialized;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="readMode">读写模式</param>
        protected DocumentBase(bool readMode)
        {
            _readMode = readMode;
        }

        DocumentHandler _initializing;
        /// <summary>
        /// 初始化开始
        /// </summary>
        public event DocumentHandler Initializing
        {
            add
            {
                _initializing += value;
            }
            remove
            {
                _initializing -= value;
            }
        }
        DocumentHandler _initialized;
        /// <summary>
        /// 初始化完成
        /// </summary>
        public event DocumentHandler Initialized
        {
            add
            {
                _initialized += value;
            }
            remove
            {
                _initialized -= value;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                throw new InfrastructureException("已经初始化，不允许重复初始化。");
            }
            FireEvent(_initializing);
            DocumentInitialize();
            FireEvent(_initialized);
            _isInitialized = true;
        }


        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void DocumentInitialize();
        

        /// <summary>
        /// 打开
        /// </summary>
        public void Open()
        {
            FireEvent(_openging);
            DocumentOpen();
            FireEvent(_opened);
        }


        /// <summary>
        /// 执行打开文档
        /// </summary>
        protected abstract void DocumentOpen();
                
        DocumentHandler _openging;
        /// <summary>
        /// 正在打开文档
        /// </summary>
        public event DocumentHandler Openging
        {
            add
            {
                _openging += value;
            }
            remove
            {
                _openging -= value;
            }
        }
        DocumentHandler _opened;
        /// <summary>
        /// 打开文档完成
        /// </summary>
        public event DocumentHandler Opened
        {
            add
            {
                _opened += value;
            }
            remove
            {
                _opened -= value;
            }
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

            FireEvent(_saving);
            DocumentSave(stream);
            FireEvent(_saving);
        }

        /// <summary>
        /// 保存文档
        /// </summary>
        /// <param name="stream">目标流</param>
        protected abstract void DocumentSave(Stream stream);
        
        DocumentHandler _saving;
        /// <summary>
        /// 正在保存文档
        /// </summary>
        public event DocumentHandler Saving
        {
            add
            {
                _saving += value;
            }
            remove
            {
                _saving -= value;
            }
        }
        DocumentHandler _saved;
        /// <summary>
        /// 文档保存完成
        /// </summary>
        public event DocumentHandler Saved
        {
            add
            {
                _saved += value;
            }
            remove
            {
                _saved -= value;
            }
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
        public T Read()
        {
            if (!_readMode)
            {
                throw new Exception("当前处理写入模式，不允许读取文档。");
            }

            FireEvent(_reading);
           T result = DocumentRead();
            FireEvent(_readed);
            return result;
        }
        DocumentHandler _reading;
        /// <summary>
        /// 读取事件
        /// </summary>
        public event DocumentHandler Reading
        {
            add
            {
                _reading += value;
            }
            remove
            {
                _reading -= value;
            }
        }
        DocumentHandler _readed;
        /// <summary>
        /// 读取完成事件
        /// </summary>
        public event DocumentHandler Readed
        {
            add
            {
                _readed += value;
            }
            remove
            {
                _readed -= value;
            }
        }
        /// <summary>
        /// 文档读取
        /// </summary>
        protected virtual T DocumentRead()
        {
            return default(T);
        }

        private void FireEvent(DocumentHandler handler)
        {
            if(handler != null)
            {
                DocumentHandler eventHandler = handler;
                handler.Invoke(this, null);
            }
        }
    }
}
