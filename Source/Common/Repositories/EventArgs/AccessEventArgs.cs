namespace Zhoubin.Infrastructure.Common.Repositories.EventArgs
{
    /// <summary>
    /// 访问事件参数
    /// </summary>
    public class AccessEventArgs : System.EventArgs
    {
        #region Constructor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="result">结果</param>
        public AccessEventArgs(bool result)
        {
            _result = result;
        }

        #endregion

        #region Result

        /// <summary>
        /// 
        /// </summary>
        private bool _result;

        /// <summary>
        /// 结果
        /// </summary>
        public bool Result
        {
            get
            {
                return _result;
            }
            set
            {
// ReSharper disable once RedundantCheckBeforeAssignment
                if (value != _result)
                {
                    _result = value;
                }
            }
        }

        #endregion
    }
}
