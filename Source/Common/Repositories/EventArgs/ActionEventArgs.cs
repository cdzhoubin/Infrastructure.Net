namespace Zhoubin.Infrastructure.Common.Repositories.EventArgs
{
    /// <summary>
    /// 动作参数
    /// </summary>
    /// <typeparam name="TEntity">事件内容类型</typeparam>
    public class ActionEventArgs<TEntity> : System.EventArgs where TEntity : class
    {
        #region Constructor

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entity">实体</param>
        public ActionEventArgs(TEntity entity)
        {
            _entity = entity;
        }

        #endregion

        #region TEntity

        /// <summary>
        /// 实体
        /// </summary>
        private readonly TEntity _entity;

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
    }
}
