using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace Zhoubin.Infrastructure.Log
{
    /// <summary>
    /// 日志基类
    /// </summary>
    public class LogEntityBase : ILogEntity
    {
        /// <summary>
        /// 日志级别
        /// </summary>
        public Sevenrity Sevenrity
        {
            get;
            set;
        }
        /// <summary>
        /// 事件编号
        /// </summary>
        public string EventId
        {
            get;
            set;
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get;
            set;
        }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get;
            set;
        }


        /// <summary>
        /// 序列化成字符串
        /// 序列化到指定的<see cref="StringBuilder"/>
        /// </summary>
        /// <param name="sb">序列化到指定的<see cref="StringBuilder"/></param>
        protected virtual void ToString(StringBuilder sb)
        {
            sb.AppendFormat("\n【EventId】{10}\n【Title】{11}\n【Content】{12}\n【CreateTime】{1}\n【Host】{0}\n【FileName】{2}\n【ThreadId】{3}\n【AppDomainName】{4}\n【PorcessId】{5}\n【ProcessName】{6}{7}\n【MachineName】{8}\n【Sevenrity】{9}"
                , HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : IPAddress
                , CreateTime
                , FileName
                , ThreadId
                , AppDomainName
                , PorcessId
                , ProcessName
                , ""
                , MachineName
                , Sevenrity
                , EventId
                , Title
                , Content);
        }

        /// <summary>
        /// 转换为字符串格式日志
        /// </summary>
        /// <returns>返回对象转换为字符串</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            ToString(sb);
            return sb.ToString();
        }

// ReSharper disable InconsistentNaming
        private volatile static string _localIP;
// ReSharper restore InconsistentNaming

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime
        {
            get { return DateTime.Now; }
        }
        /// <summary>
        /// Ip地址
        /// </summary>
        public string IPAddress
        {
            get
            {
                if (_localIP != null)
                {
                    return _localIP;
                }

                lock (this)
                {
                    switch (_localIP)
                    {
                        case null:
                        {
// ReSharper disable InconsistentNaming
                            var localIP = "";
// ReSharper restore InconsistentNaming
                            var hostName = Dns.GetHostName(); //得到主机名
                            var ipEntry = Dns.GetHostEntry(hostName);
                            foreach (var t in ipEntry.AddressList.Where(t => t.AddressFamily == AddressFamily.InterNetwork))
                            {
                                localIP = t.ToString();
                                break;
                            }

                            _localIP = localIP;
                        }
                            break;
                    }
                }

                return _localIP;
            }
        }
        /// <summary>
        /// 启动进程名
        /// </summary>
        public string FileName
        {
            get { return Process.GetCurrentProcess().StartInfo.FileName; }
        }
        /// <summary>
        /// 线程编号
        /// </summary>
        public int ThreadId
        {
            get { return System.Threading.Thread.CurrentThread.ManagedThreadId; }
        }
        /// <summary>
        /// 应用程序域名
        /// </summary>
        public string AppDomainName
        {
            get { return AppDomain.CurrentDomain.FriendlyName; }
        }
        /// <summary>
        /// 进程编号
        /// </summary>
        public int PorcessId
        {
            get { return Process.GetCurrentProcess().Id; }
        }
        /// <summary>
        /// 进程名称
        /// </summary>
        public string ProcessName
        {
            get { return Process.GetCurrentProcess().ProcessName; }
        }

        /// <summary>
        /// 机构名
        /// </summary>
        public string MachineName
        {
            get { return Process.GetCurrentProcess().MachineName; }
        }

    }
}