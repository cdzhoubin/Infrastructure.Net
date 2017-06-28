using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForm.Extent
{
    public static class WinFormExtent
    {
        #region

        private static Mutex _mutex;
        #endregion
        #region 方法四：使用的Win32函数的声明

        /// <summary>
        /// 找到某个窗口与给出的类别名和窗口名相同窗口
        /// 非托管定义为：http://msdn.microsoft.com/en-us/library/windows/desktop/ms633499(v=vs.85).aspx
        /// </summary>
        /// <param name="lpClassName">类别名</param>
        /// <param name="lpWindowName">窗口名</param>
        /// <returns>成功找到返回窗口句柄,否则返回null</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 切换到窗口并把窗口设入前台,类似 SetForegroundWindow方法的功能
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="fAltTab">True代表窗口正在通过Alt/Ctrl +Tab被切换</param>
        [DllImport("user32.dll ", SetLastError = true)]
        static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        ///// <summary>
        /////  设置窗口的显示状态
        /////  Win32 函数定义为：http://msdn.microsoft.com/en-us/library/windows/desktop/ms633548(v=vs.85).aspx
        ///// </summary>
        ///// <param name="hWnd">窗口句柄</param>
        ///// <param name="cmdShow">指示窗口如何被显示</param>
        ///// <returns>如果窗体之前是可见，返回值为非零；如果窗体之前被隐藏，返回值为零</returns>
        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        // ReSharper disable once InconsistentNaming
        const int SW_RESTORE = 9;
        // ReSharper disable once InconsistentNaming
        static IntPtr formhwnd;
        #endregion

        /// <summary>
        /// 检查信号量是否已经运行
        /// 如果没有存在，执行显示窗体Action:showWindowAction
        /// </summary>
        /// <param name="name">互斥变量</param>
        /// <param name="showWindowAction">显示窗口<see cref="Action"/></param>
        /// <param name="showDoubleRunAction">显示重复执行<see cref="Action"/></param>
        public static void RunSingleForm(this string name, Action showWindowAction, Action showDoubleRunAction = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (showWindowAction == null)
            {
                throw new ArgumentNullException("showWindowAction");
            }

            if (_mutex != null)
            {
                return;
            }

            bool createNew;
            _mutex = new Mutex(true, name, out createNew);
            if (createNew)
            {
                showWindowAction();
                return;
            }

            if (showDoubleRunAction == null)
            {
                showDoubleRunAction = () => MessageBox.Show("此程序实例已经运行，请不要重复运行。");
            }

            showDoubleRunAction();

            ActiveWindow();
            Thread.Sleep(1000);
            Application.Exit();
        }

        private static void ActiveWindow()
        {
            #region 方法四: 可以是托盘中的隐藏程序显示出来

            // 方法四相对于方法三而言应该可以说是一个改进，
            // 因为方法三只能是最小化的窗体显示出来，如果隐藏到托盘中则不能把运行的程序显示出来
            // 具体问题可以看这个帖子：http://social.msdn.microsoft.com/Forums/zh-CN/6398fb10-ecc2-4c03-ab25-d03544f5fcc9
            Process currentproc = System.Diagnostics.Process.GetCurrentProcess();
            Process[] processcollection =
                System.Diagnostics.Process.GetProcessesByName(currentproc.ProcessName.Replace(".vshost", string.Empty));
            //  该程序已经运行，
            if (processcollection.Length >= 1)
            {
                foreach (Process process in processcollection)
                {
                    if (process.Id != currentproc.Id)
                    {
                        // 如果进程的句柄为0，即代表没有找到该窗体，即该窗体隐藏的情况时
                        if (process.MainWindowHandle.ToInt32() == 0)
                        {
                            // 获得窗体句柄
                            formhwnd = FindWindow(null, Application.ProductName);
                            // 重新显示该窗体并切换到带入到前台
                            ShowWindow(formhwnd, SW_RESTORE);
                            SwitchToThisWindow(formhwnd, true);
                        }
                        else
                        {
                            // 如果窗体没有隐藏，就直接切换到该窗体并带入到前台
                            // 因为窗体除了隐藏到托盘，还可以最小化
                            SwitchToThisWindow(process.MainWindowHandle, true);
                        }
                    }
                }
            }

            #endregion
        }
    }
}
