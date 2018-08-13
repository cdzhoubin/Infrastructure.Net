using System;
using MongoDB.Bson;

namespace Zhoubin.Infrastructure.Common.MongoDb.Entity
{
    /// <summary>
    /// 文件元数据基类
    /// </summary>
    public abstract class FileEntity: EntityBase, IMetaEntity
    {

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// Hash编码
        /// </summary>
        public string HashCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 文件存储的数据库名称，默认值为空，表示存储当前数据库
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// 实体Hash
        /// </summary>
        public int EntityHash
        {
            get { return CollectionName.GetHashCode(); }
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }

        /// <summary>
        /// 编号
        /// </summary>
        public ObjectId Id
        {
            get; set;
        }

        /// <summary>
        /// 填充实体
        /// </summary>
        /// <param name="entity">填充实体</param>
        public virtual void Fill(IEntity entity)
        {
            var e = entity as FileEntity;
            if (e == null)
            {
                return;
            }

            FileName = e.FileName;
            ContentType = e.ContentType;
            FileSize = e.FileSize;
            HashCode = e.HashCode;
            Name = e.Name;
            Description = e.Description;
        }
    }
    /// <summary>
    /// 文件存储基类
    /// </summary>
    [Obsolete("不推荐使用此类实现，现在只提供兼容性，请使用FileEntity替代。")]
    public abstract class FileEntityBase : FileEntity
    {
        /// <summary>
        /// 使得对象填充当前对象
        /// </summary>
        /// <param name="entity">填充实体</param>
        public override void Fill(IEntity entity)
        {
            base.Fill(entity);
            var e = entity as FileEntityBase;
            if (e == null)
            {
                return;
            }

            CreateName = e.CreateName;
            CreateTime = e.CreateTime;
            ModifyName = e.ModifyName;
            ModifyTime = e.ModifyTime;
        }
        private DateTime _createTime;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get { return _createTime.ToLocalTime(); } set { _createTime = value.ToUniversalTime(); } }
        private DateTime _modifyTime;
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime { get { return _modifyTime.ToLocalTime(); } set { _modifyTime = value.ToUniversalTime(); } }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateName { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string ModifyName { get; set; }
    }
}