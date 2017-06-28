namespace Zhoubin.Infrastructure.Common.Repositories.EventArgs
{
    /// <summary>
    /// 过虑参数
    /// </summary>
    /// <typeparam name="TEntity">泛型类型</typeparam>
    public class FilterEventArgs<TEntity> : System.EventArgs where TEntity : class
    {
        #region Constructor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="result">结果</param>
        public FilterEventArgs(TEntity entity, bool result)
        {
            _entity = entity;
            _result = result;
        }

        #endregion

        #region TEntity

        /// <summary>
        /// 
        /// </summary>
        private TEntity _entity;

        /// <summary>
        /// 实体
        /// </summary>
        public TEntity Entity
        {
            get
            {
                return _entity;
            }
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
                if (value != _result)
                {
                    _result = value;
                }
            }
        }

        #endregion
    }
}
