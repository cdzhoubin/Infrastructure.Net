using MongoDB.Bson;
using Zhoubin.Infrastructure.Common.MongoDb;
using Zhoubin.Infrastructure.Common.MongoDb.Entity;

namespace Zhoubin.Infrastructure.Common.Identity.MongoDb
{
    /// <summary>
    /// 用户登录记录
    /// </summary>
    public class UserLogins : DocumentEntity
    {
        /// <summary>
        /// 登录提供者
        /// </summary>
        public string LoginProvider { get; set; }
        /// <summary>
        /// 提供者标识
        /// </summary>
        public string ProviderKey { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public ObjectId UserId { get; set; }

        /// <inheritdoc />
        public override string CollectionName
        {
            get { return "UserLogins"; }
        }

        /// <inheritdoc />
        public override void Fill(IEntity entity)
        {
            var newEntity = entity as UserLogins;
            if (newEntity != null)
            {
                LoginProvider = newEntity.LoginProvider;
                ProviderKey = newEntity.ProviderKey;
                UserId = newEntity.UserId;
            }

            base.Fill(entity);
        }
    }
}