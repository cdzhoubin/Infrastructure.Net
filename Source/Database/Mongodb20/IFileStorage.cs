using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;

namespace Zhoubin.Infrastructure.Common.MongoDb
{
    public interface IFileStorage : IDisposable
    {

        /// <summary>
        /// 新增文件
        /// </summary>
        /// <param name="context">文件内容</param>
        /// <param name="entity">文件元数据</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回对象标识</returns>
        ObjectId Insert<T>(Stream context, T entity) where T : IMetaEntity, new();

        /// <summary>
        /// 更新指定文件的元数据
        /// </summary>
        /// <param name="entity">对象</param>
        /// <typeparam name="T">表类型</typeparam>
        void Update<T>(T entity) where T : IMetaEntity, new();

        /// <summary>
        /// 删除指定Id的文件
        /// </summary>
        /// <param name="id">标识</param>
        /// <typeparam name="T">表类型</typeparam>
        void Delete<T>(ObjectId id) where T : IMetaEntity, new();
        /// <summary>
        /// 根据指定的条件删除文件
        /// </summary>
        /// <param name="condition">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        void Delete<T>(IDictionary<string, object> condition) where T : IMetaEntity, new();

        /// <summary>
        /// 根据指定的Id查询文件元数据
        /// </summary>
        /// <param name="id">标识</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回查询结果</returns>
        T FindById<T>(ObjectId id) where T : IMetaEntity, new();
        /// <summary>
        /// 根据指定的Id查询文件元数据
        /// </summary>
        /// <param name="id">标识</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回查询结果</returns>
        T FindById<T>(string id) where T : IMetaEntity, new();
        /// <summary>
        /// 根据条件查找第一个满足条件的文件
        /// </summary>
        /// <param name="condition">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回对象</returns>
        T FindOneByCondition<T>(IDictionary<string, object> condition) where T : IMetaEntity, new();

        /// <summary>
        /// 检查指定条件的文件是否存在
        /// </summary>
        /// <param name="condition">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>true:存在</returns>
        bool Any<T>(IDictionary<string, object> condition) where T : IMetaEntity, new();

        /// <summary>
        /// 检查指定条件的文件是否存在
        /// </summary>
        /// <param name="condition">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>true:存在</returns>
        bool Any<T>(Expression<Func<T, bool>> where) where T : IMetaEntity, new();

        /// <summary>
        /// 查询文件列表
        /// </summary>
        /// <param name="condition">查询键值条件</param>
        /// <param name="sortList">排序字典</param>
        /// <typeparam name="T">返回对象类型</typeparam>
        /// <returns>查询结果</returns>
        List<T> FindByCondition<T>(IDictionary<string, object> condition, IDictionary<string, bool> sortList) where T : IMetaEntity, new();

        /// <summary>
        /// 查询文件对象列表
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="isAsc">是否升序</param>
        /// <typeparam name="T">返回类型</typeparam>
        /// <typeparam name="TOrderBy">排序类型</typeparam>
        /// <returns>返回指定类型对象</returns>
        List<T> FindByQuery<T, TOrderBy>(Expression<Func<T, bool>> where, Expression<Func<T, TOrderBy>> orderby,
                                         bool isAsc) where T : IMetaEntity, new();

        /// <summary>
        /// 查询文件对象列表
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <param name="isAsc">是否升序</param>
        /// <typeparam name="T">返回类型</typeparam>
        /// <typeparam name="TOrderBy">排序类型</typeparam>
        /// <returns>返回指定类型对象</returns>
        List<T> FindByQuery<T>(Expression<Func<T, bool>> where) where T : IMetaEntity, new();

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
        PageInfo<T> FindByQuery<T, TOrderBy>(int index, int pageSize, Expression<Func<T, bool>> where,
            Expression<Func<T, TOrderBy>> orderby, bool isAsc) where T : IMetaEntity, new();

        /// <summary>
        /// 下载指定标识的文件
        /// </summary>
        /// <param name="id">标识</param>
        /// <returns>返回流</returns>
        Stream DownLoad(ObjectId id);

        /// <summary>
        /// 下载指定标识的文件
        /// </summary>
        /// <param name="id">标识</param>
        /// <param name="saveFile">保存文件路径</param>
        void DownLoad(ObjectId id, string saveFile);
        
        /// <summary>
        /// 下载指定标识的文件
        /// </summary>
        /// <typeparam name="T">文件元数据类型</typeparam>
        /// <param name="id">标识</param>
        /// <returns>返回流</returns>
        Stream DownLoad<T>(ObjectId id) where T : IMetaEntity, new();

        /// <summary>
        /// 下载指定标识的文件
        /// </summary>
        /// <typeparam name="T">文件元数据类型</typeparam>
        /// <param name="id">标识</param>
        /// <param name="saveFile">保存文件路径</param>
        void DownLoad<T>(ObjectId id, string saveFile) where T : IMetaEntity, new();

        /// <summary>
        /// 下载指定标识的文件
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="saveFile">保存文件路径</param>
        void DownLoad<T>(IDictionary<string, object> condition, string saveFile) where T : IMetaEntity, new();

        /// <summary>
        /// 根据指定条件下载查询到的第一个文件
        /// 用于文档型数据查询
        /// </summary>
        /// <param name="condition">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>true:存在</returns>
        Stream DownLoad<T>(IDictionary<string, object> condition) where T : IMetaEntity, new();
       

        #region download Linq
        /// <summary>
        /// 下载指定标识的文件
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="saveFile">保存文件路径</param>
        void DownLoad<T>(Expression<Func<T, bool>> where, string saveFile) where T : IMetaEntity, new();

        /// <summary>
        /// 根据指定条件下载查询到的第一个文件
        /// 用于文档型数据查询
        /// </summary>
        /// <param name="condition">条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>true:存在</returns>
        Stream DownLoad<T>(Expression<Func<T, bool>> where) where T : IMetaEntity, new();

        /// <summary>
        /// 下载指定标识的文件
        /// </summary>
        /// <typeparam name="T">文件元数据类型</typeparam>
        /// <param name="id">标识</param>
        /// <returns>返回流</returns>
        Stream DownLoad<T>(string id) where T : IMetaEntity, new();

        /// <summary>
        /// 下载指定标识的文件
        /// </summary>
        /// <typeparam name="T">文件元数据类型</typeparam>
        /// <param name="id">标识</param>
        /// <param name="saveFile">保存文件路径</param>
        void DownLoad<T>(string id, string saveFile) where T : IMetaEntity, new();
        /// <summary>
        /// 下载指定标识的文件
        /// </summary>
        /// <param name="id">标识</param>
        /// <returns>返回流</returns>
        Stream DownLoad(string id);
        #endregion

        /// <summary>
        /// 删除所有文件
        /// </summary>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>操作成功返回true</returns>
        bool DeleteAll<T>() where T : IMetaEntity, new();
        /// <summary>
        /// 查询文件对象列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回指定类型对象</returns>
        List<T> AdvanceQuery<T>(Func<IQueryable<T>, IQueryable<T>> query, Func<IQueryable<T>, IQueryable<T>> orderby) where T : IMetaEntity, new();

        /// <summary>
        /// 分页查询文件列表
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="query">查询条件</param>
        /// <param name="orderby">排序条件</param>
        /// <typeparam name="T">表类型</typeparam>
        /// <returns>返回指定类型对象</returns>
        PageInfo<T> AdvanceQuery<T>(int index, int pageSize, Func<IQueryable<T>, IQueryable<T>> query, Func<IQueryable<T>, IQueryable<T>> orderby) where T : IMetaEntity, new();

    }
}
