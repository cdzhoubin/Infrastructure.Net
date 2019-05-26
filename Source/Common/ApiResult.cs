using System.Collections.Generic;

namespace Zhoubin.Infrastructure.Common
{

    /// <summary>
    /// 应用返回对象
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class ResultApi<TData> : ResultApi
    {
        /// <summary>
        /// 执行成功返回数据集
        /// </summary>
        public TData Data { get; internal set; }
    }
    /// <summary>
    /// 返回结果
    /// </summary>
    public class ResultApi : ResultBase<List<ModelErrorPair>>
    {

        /// <summary>
        /// 创建成功对象
        /// </summary>
        /// <returns></returns>
        public static ResultApi Sucess()
        {
            return new ResultApi { IsSucess = true, ErrorMessage = new List<ModelErrorPair>() };
        }
        /// <summary>
        /// 创建执行成功对象
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static ResultApi<TData> Sucess<TData>(TData data)
        {
            return new ResultApi<TData> { IsSucess = true, ErrorMessage = new List<ModelErrorPair>(), Data = data };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <returns></returns>
        public static ResultApi Error(List<ModelErrorPair> message)
        {
            return new ResultApi { IsSucess = false, ErrorMessage = message };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <returns></returns>
        public static ResultApi Error(string message)
        {
            return Error(new List<ModelErrorPair> { new ModelErrorPair("", message) });
        }
        /// <summary>
        /// 出错对象创建
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="message">错误信息</param>
        /// <returns></returns>
        public static ResultApi<TData> Error<TData>(string message)
        {
            return Error<TData>(new List<ModelErrorPair> { new ModelErrorPair("", message) });
        }
        /// <summary>
        /// 出错对象创建
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="message">错误信息</param>
        /// <returns></returns>
        public static ResultApi<TData> Error<TData>(List<ModelErrorPair> message)
        {
            return new ResultApi<TData> { IsSucess = false, ErrorMessage = message };
        }
    }
    /// <summary>
    /// Api模型校验错误
    /// </summary>
    public class ModelErrorPair
    {
        public ModelErrorPair() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">字段名称</param>
        /// <param name="message">错误内容</param>
        public ModelErrorPair(string key, string message)
        {
            Key = key;
            Message = message;
        }
        /// <summary>
        /// 字段名称
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 错误内容
        /// </summary>
        public string Message { get; set; }
    }
}
