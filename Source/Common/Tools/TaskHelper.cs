using System;
using System.Threading.Tasks;

namespace Zhoubin.Infrastructure.Common.Tools
{
    public static class TaskHelper
    {
        /// <summary>
        /// 操作运行
        /// </summary>
        /// <param name="action">操作</param>
        /// <returns>返回操作对象</returns>
        public static Task RunTask(Action action)
        {
            var task = new Task(action);
            task.Start();
            return task;
        }

        /// <summary>
        /// 操作运行
        /// </summary>
        /// <param name="action">操作</param>
        /// <typeparam name="T">操作返回类型</typeparam>
        /// <returns>返回操作对象</returns>
        public static Task<T> RunTask<T>(Func<T> action)
        {
            var task = new Task<T>(action);
            task.Start();
            return task;
        }
    }
}
