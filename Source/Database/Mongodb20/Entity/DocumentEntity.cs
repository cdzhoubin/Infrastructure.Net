using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Zhoubin.Infrastructure.Common.Extent;

namespace Zhoubin.Infrastructure.Common.MongoDb.Entity
{
    /// <summary>
    /// 文档存储基类
    /// </summary>
    public abstract class DocumentEntity : EntityBase, IDocumentEntity
    {
        /// <summary>
        /// 对象编号
        /// </summary>
        public ObjectId Id
        {
            get;
            set;
        }
        /// <summary>
        /// 创建本实体的引用链接
        /// </summary>
        /// <param name="databaseName">数据库名称</param>
        /// <returns>返回生成的引用链接</returns>
        public MongoDBRef CreateDbRef(string databaseName = null)
        {
            if (Id == ObjectId.Empty)
            {
                throw new MongoException("对象实例没有ObjectId，无法创建引用对象。");
            }

            return string.IsNullOrEmpty(databaseName) ? new MongoDBRef(CollectionName, Id) : new MongoDBRef(databaseName, CollectionName, Id);
        }

        /// <summary>
        /// 填充方法
        /// </summary>
        /// <param name="entity">填充对象</param>
        public virtual void Fill(IEntity entity)
        {

        }

        internal static readonly Dictionary<Type, string> DicCollectionName = new Dictionary<Type, string>();
        internal static readonly Hashtable SyncSign = new Hashtable();
        internal static readonly Dictionary<Type, List<PropertyInfo>> DicCollectionPropertyInfo = new Dictionary<Type, List<PropertyInfo>>();
        /// <summary>
        /// 通过反射初始化类型列表
        /// </summary>
        /// <param name="type"></param>
        internal static void InitType(Type type)
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (var field in type.GetProperties())
            {
                if (field.Name == "Id")
                {
                    continue;
                }
                if (field.GetMethod == null || !field.GetMethod.IsPublic)
                {
                    continue;
                }
                if (field.SetMethod == null || !field.SetMethod.IsPublic)
                {
                    continue;
                }
                list.Add(field);
            }
            DicCollectionPropertyInfo.Add(type, list);
            DicCollectionName.Add(type, type.FullName.Substring(type.FullName.LastIndexOf(".", StringComparison.Ordinal) + 1).ToPlural());
        }
    }
    /// <summary>
    /// 文档存储基类
    /// 使用类型名作为存储CollectionName
    /// 同时使用反射实现Fill方法
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DocumentEntity<TEntity> : DocumentEntity where TEntity : DocumentEntity
    {
        /// <summary>
        /// 存储表名称
        /// </summary>
        public override string CollectionName
        {
            get
            {
                Type type = typeof(TEntity);
                if (!DicCollectionName.ContainsKey(type))
                {
                    lock (SyncSign.SyncRoot)
                    {
                        if (!DicCollectionName.ContainsKey(type))
                        {
                            InitType(type);
                        }
                    }
                }

                return DicCollectionName[type];
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public override void Fill(IEntity entity)
        {
            TEntity newEntity = entity as TEntity;
            if (newEntity != null)
            {
                Fill(newEntity);
            }
            base.Fill(entity);
        }
        /// <summary>
        /// 使用反射实现属性映射
        /// </summary>
        /// <param name="entity"></param>
        protected void Fill(TEntity entity)
        {
            Type type = typeof(TEntity);
            if (!DicCollectionPropertyInfo.ContainsKey(type))
            {
                lock (SyncSign.SyncRoot)
                {
                    if (!DicCollectionPropertyInfo.ContainsKey(type))
                    {
                        InitType(type);
                    }
                }
            }
            foreach (var field in DicCollectionPropertyInfo[type])
            {
                field.SetValue(this, field.GetValue(entity));
            }
        }
    }
}
