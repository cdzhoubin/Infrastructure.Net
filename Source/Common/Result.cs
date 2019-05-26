using System;
using System.Collections.Generic;
using System.Text;

namespace Zhoubin.Infrastructure.Common
{
    public abstract class ResultBase<TError>
    {
        /// <summary>
        /// 成功返回true,其它返回false
        /// </summary>
        public bool IsSucess { get; protected set; }
        /// <summary>
        /// 当出错时，返回错误描述信息
        /// </summary>
        public TError ErrorMessage { get; protected set; }
    }
    /// <summary>
    /// 返回结果
    /// </summary>
    public class Result:ResultBase<string>
    {
        
        /// <summary>
        /// 创建成功对象
        /// </summary>
        /// <returns></returns>
        public static Result Sucess()
        {
            return new Result { IsSucess = true, ErrorMessage = "" };
        }
        /// <summary>
        /// 创建执行成功对象
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static Result<TData> Sucess<TData>(TData data)
        {
            return new Result<TData> { IsSucess = true, ErrorMessage = "", Data = data };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">错误信息</param>
        /// <returns></returns>
        public static Result Error(string message)
        {
            return new Result { IsSucess = false, ErrorMessage = message };
        }
        /// <summary>
        /// 出错对象创建
        /// </summary>
        /// <typeparam name="TData">数据类型</typeparam>
        /// <param name="message">错误信息</param>
        /// <returns></returns>
        public static Result<TData> Error<TData>(string message)
        {
            return new Result<TData> { IsSucess = false, ErrorMessage = message };
        }
    }
    /// <summary>
    /// 应用返回对象
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class Result<TData> : Result
    {
        /// <summary>
        /// 执行成功返回数据集
        /// </summary>
        public TData Data { get; internal set; }
    }
}
