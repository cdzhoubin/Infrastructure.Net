using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Driver.GridFS;

namespace Zhoubin.Infrastructure.Common.MongoDb
{
    /// <summary>
    /// 文件存储接口
    /// </summary>
    public interface IFileStorage: IFileStorage20
    {
        /// <summary>
        /// 查询文件对象列表
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="isAsc">是否升序</param>
        /// <typeparam name="T">返回类型</typeparam>
        /// <typeparam name="TOrderBy">排序类型</typeparam>
        /// <returns>返回指定类型对象</returns>
        List<T> FindByQuery<T, TOrderBy>(Expression<Func<MongoGridFSFileInfo, bool>> where, Expression<Func<MongoGridFSFileInfo, TOrderBy>> orderby,
                                         bool isAsc) where T : IMetaEntity, new();
        /// <summary>
        /// 分页查询文件列表
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="isAsc">是否升序</param>
        /// <typeparam name="T">返回类型</typeparam>
        /// <typeparam name="TOrderBy">排序类型</typeparam>
        /// <returns>返回指定类型对象</returns>
        PageInfo<T> FindByQuery<T, TOrderBy>(int index, int pageSize, Expression<Func<MongoGridFSFileInfo, bool>> where,
            Expression<Func<MongoGridFSFileInfo, TOrderBy>> orderby, bool isAsc) where T : IMetaEntity, new();

        

        


        /// <summary>
        /// 查询文件对象列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回指定类型对象</returns>
        List<T> AdvanceQuery<T>(Func<IQueryable<MongoGridFSFileInfo>, IQueryable<MongoGridFSFileInfo>> query, Func<IQueryable<MongoGridFSFileInfo>, IQueryable<MongoGridFSFileInfo>> orderby) where T : IMetaEntity, new();

        /// <summary>
        /// 分页查询文件列表
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="query">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回指定类型对象</returns>
        PageInfo<T> AdvanceQuery<T>(int index, int pageSize, Func<IQueryable<MongoGridFSFileInfo>, IQueryable<MongoGridFSFileInfo>> query, Func<IQueryable<MongoGridFSFileInfo>, IQueryable<MongoGridFSFileInfo>> orderby) where T : IMetaEntity, new();


        
    }
}
