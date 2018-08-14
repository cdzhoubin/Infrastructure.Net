using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using Zhoubin.Infrastructure.Common.MongoDb;

namespace Zhoubin.Infrastructure.Common.Identity.MongoDb
{
    /// <summary>
    /// Class that implements the ASP.NET Identity
    /// 角色存储定义 
    /// </summary>
    public class MongodbIdentityRole : IdentityRole<ObjectId>, IDocumentEntity
    {
        /// <summary>
        /// 构造函数 
        /// </summary>
        public MongodbIdentityRole()
        {
            Id = ObjectId.GenerateNewId();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">角色名称</param>
        public MongodbIdentityRole(string name)
            : this()
        {
            Name = name;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">角色名称</param>
        /// <param name="id">编号</param>
        public MongodbIdentityRole(string name, string id)
        {
            Name = name;
            Id = new ObjectId(id);
        }

        
        /// <summary>
        /// 角色存储数据库名称
        /// </summary>
        public string CollectionName
        {
            get { return "Roles"; }
        }

        public void Fill(IEntity entity)
        {
            if(entity == null)
            {
                return;
            }
            var result = entity as IdentityRole;
            if(result == null)
            {
                return;
            }
            Name = result.Name;
            NormalizedName = result.NormalizedName;
            ConcurrencyStamp = result.ConcurrencyStamp;
        }
    }
}
