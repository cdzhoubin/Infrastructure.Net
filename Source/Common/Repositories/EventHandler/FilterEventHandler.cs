using System;
using System.Runtime.InteropServices;
using Zhoubin.Infrastructure.Common.Repositories.EventArgs;

namespace Zhoubin.Infrastructure.Common.Repositories.EventHandler
{
    /// <summary>
    /// 数据过虑事件
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <param name="sender">发送者</param>
    /// <param name="e">事件</param>
    [Serializable]
    [ComVisible(true)]
    public delegate void FilterEventHandler<TEntity>(object sender, FilterEventArgs<TEntity> e) where TEntity : class;
}
