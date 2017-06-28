using System;
using System.Runtime.InteropServices;
using Zhoubin.Infrastructure.Common.Repositories.EventArgs;

namespace Zhoubin.Infrastructure.Common.Repositories.EventHandler
{
    /// <summary>
    /// 提交事件委托
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="e">提交事件参数</param>
    [Serializable]
    [ComVisible(true)]
    public delegate void CommitEventHandler(object sender, CommitEventArgs e);
}
