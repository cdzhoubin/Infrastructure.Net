using System.Collections.Generic;
using System.Security.Claims;
using MongoDB.Bson;
using Zhoubin.Infrastructure.Common.MongoDb;
using Zhoubin.Infrastructure.Common.MongoDb.Entity;

namespace Zhoubin.Infrastructure.Common.Identity.MongoDb
{
    /// <summary>
    /// UserClaims
    /// </summary>
    public class UserClaims : DocumentEntity
    {
        /// <summary>
        /// Claim
        /// </summary>
        public Claim Claim { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public ObjectId UserId { get; set; }

        /// <inheritdoc />
        public override string CollectionName
        {
            get { return "UserClaims"; }
        }

        /// <inheritdoc />
        public override void Fill(IEntity entity)
        {
            var newEntity = entity as UserClaims;
            if (newEntity != null)
            {
                Claim = newEntity.Claim;
                UserId = newEntity.UserId;
            }

            base.Fill(entity);
        }
    }
}