using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Zhoubin.Infrastructure.Common.MongoDb;
using Zhoubin.Infrastructure.Common.MongoDb.Entity;

namespace Zhoubin.Infrastructure.Common.Identity.MongoDb
{
    /// <summary>
    /// 泛型用户实现
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    public class MongodbIdentityUser<TRole> : MongodbIdentityUser where TRole : IdentityRole<ObjectId>, new()
    {
        /// <summary>
        /// 构造函数
        /// </summary>

        public MongodbIdentityUser()
        {
            Roles = new List<TRole>();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userName">用户名</param>
        public MongodbIdentityUser(string userName)
        {
            UserName = userName;
            Roles = new List<TRole>();
        }
        /// <summary>
        /// 用户角色
        /// </summary>
        [BsonIgnore]
        public new List<TRole> Roles { get; set; }


        /// <inheritdoc />
        public override void Fill(IEntity entity)
        {
            base.Fill(entity);
            var newEntity = entity as MongodbIdentityUser;
            if (newEntity != null)
            {
                if (!string.IsNullOrEmpty(newEntity.UserName))
                {
                    UserName = newEntity.UserName;
                }
                if (!string.IsNullOrEmpty(newEntity.PasswordHash))
                {
                    PasswordHash = newEntity.PasswordHash;
                }
                if (!string.IsNullOrEmpty(newEntity.SecurityStamp))
                {
                    SecurityStamp = newEntity.SecurityStamp;
                }

                if (newEntity.Roles != null && newEntity.Roles.Count > 0)
                {
                    Roles = new List<TRole>();
                    newEntity.Roles.ForEach(p => Roles.Add(new TRole() { Name = p.Name, Id = new ObjectId( p.Id) }));
                }
            }

        }
    }
    /// <summary>
    /// 用户类型
    /// </summary>
    public class MongodbIdentityUser : IdentityUser<ObjectId>,IDocumentEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MongodbIdentityUser()
        {
            Roles = new List<IdentityRole>();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userName">用户名</param>
        public MongodbIdentityUser(string userName)
        {
            UserName = userName;
            Roles = new List<IdentityRole>();
        }

        
        /// <summary>
        /// 登录失败次数
        /// </summary>
        public int LoginFail { get; set; }
        /// <summary>
        /// 锁定截止日期
        /// </summary>
        public DateTimeOffset? LockoutEndDate { get; set; }
        /// <inheritdoc />
        public string CollectionName
        {
            get { return "Users"; }
        }
        /// <summary>
        /// 用户角色
        /// </summary>
        [BsonIgnore]
        public List<IdentityRole> Roles { get; set; }

        //string IUser.UserName
        //{
        //    get
        //    {
        //        return UserName;
        //    }
        //    set
        //    {
        //        UserName = value;
        //    }
        //}

        /// <inheritdoc />
        public virtual void Fill(IEntity entity)
        {
            var newEntity = entity as MongodbIdentityUser;
            if (newEntity != null)
            {
                if (!string.IsNullOrEmpty(newEntity.UserName))
                {
                    UserName = newEntity.UserName;
                }
                if (!string.IsNullOrEmpty(newEntity.PasswordHash))
                {
                    PasswordHash = newEntity.PasswordHash;
                }
                if (!string.IsNullOrEmpty(newEntity.SecurityStamp))
                {
                    SecurityStamp = newEntity.SecurityStamp;
                }
                
                LockoutEndDate = newEntity.LockoutEndDate;
                LoginFail = newEntity.LoginFail;

                if (newEntity.Roles != null && newEntity.Roles.Count > 0)
                {
                    Roles = new List<IdentityRole>();
                    newEntity.Roles.ForEach(p => Roles.Add(new IdentityRole() { Name = p.Name, Id = p.Id }));
                }
            }
        }
    }

    
}
