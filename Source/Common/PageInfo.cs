using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zhoubin.Infrastructure.Common
{
    /// <summary>
    /// 分页查询信息类
    /// </summary>
    public class PageInfo
    {
        /// <summary>
        /// 检查分页索引和分页大小是否满足边界条件
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="size">大小</param>
        public static void CheckPageIndexAndSize(ref int index, ref int size)
        {
            if (index < 1)
            {
                index = 1;
            }

            if (size < 1)
            {
                size = 20;
            }
        }

        /// <summary>
        /// 根据记录总数修正当前查询的分页索引
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="size">大小</param>
        /// <param name="count">记录总数</param>
        public static void CheckPageIndexAndSize(ref int index, int size, int count)
        {
            if (count >= index * size)
            {
                return;
            }

            index = count / size;
            if (count % size > 0)
            {
                index++;
            }

            if (index == 0)
            {
                index = 1;
            }
        }

    }

    /// <summary>
    /// 分页查询信息泛型类
    /// </summary>
    /// <typeparam name="T">查询出的数据类型</typeparam>
    public class PageInfo<T> : PageInfo
    {
        internal PageInfo()
        {
            DataList = new ReadOnlyCollection<T>(new T[0]);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="index">索引，从1开始记数</param>
        /// <param name="pageSize">大小</param>
        /// <param name="count">总数</param>
        /// <param name="dataList">数据</param>
        public PageInfo(int index, int pageSize, int count, List<T> dataList)
        {
            Index = index;
            PageSize = pageSize;
            Count = count;
            DataList = new ReadOnlyCollection<T>(dataList);
        }

        /// <summary>
        /// 页面索引
        /// </summary>
        public int Index { get; private set; }
        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; private set; }
        /// <summary>
        /// 记录总数
        /// </summary>
        public int Count { get; private set; }
        /// <summary>
        /// 数据集信息
        /// </summary>
        public ReadOnlyCollection<T> DataList { get; private set; }

        /// <summary>
        /// 空
        /// </summary>
        public static PageInfo<T> Empty
        {
            get { return new PageInfo<T>(); }
        }
    }
}
