using MongoDB.Bson;
using Zhoubin.Infrastructure.Common.MongoDb.Entity;

namespace Zhoubin.Infrastructure.Common.Identity.MongoDb
{
    /// <summary>
    /// 用户角色关系表
    /// </summary>
    public class UserRole: DocumentEntity
    {
        /// <inheritdoc />
        public override string CollectionName { get { return "URRel"; } }
        /// <summary>
        /// 用户编号
        /// </summary>
        public ObjectId UserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 角色编号
        /// </summary>
        public ObjectId RoleId { get; set; }
        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }
    }
}