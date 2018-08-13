using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Zhoubin.Infrastructure.Common.MongoDb.Entity
{
    /// <summary>
    /// 文档对象存储基类
    /// </summary>
    [Obsolete("此类型已经不推荐使用，请使用DocumentEntity代替")]
    public abstract class DocumentEntityBase : DocumentEntity
    {
        /// <summary>
        /// 使得对象填充当前对象
        /// </summary>
        /// <param name="entity">填充实体</param>
        public override void Fill(IEntity entity)
        {
            var e = entity as DocumentEntityBase;
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
