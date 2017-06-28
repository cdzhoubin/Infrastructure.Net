using System;
using System.Runtime.InteropServices;
using Zhoubin.Infrastructure.Common.Repositories.EventArgs;

namespace Zhoubin.Infrastructure.Common.Repositories.EventHandler
{
    /// <summary>
    /// 访问事件Handle
    /// </summary>
    /// <param name="sender">发送者</param>
    /// <param name="e">参数</param>
    [Serializable]
    [ComVisible(true)]
    public delegate void AccessEventHandler(object sender, AccessEventArgs e);
}
