using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using Zhoubin.Infrastructure.Common.MongoDb;

namespace Zhoubin.Infrastructure.Common.Identity.MongoDb
{
    /// <summary>
    /// 两阶段验证用户定义
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    public class TwoFactorIdentityUser<TRole> : TwoFactorIdentityUser
    {
        /// <summary>
        /// 构造函数
        /// </summary>

        public TwoFactorIdentityUser()
        {
            Roles = new List<TRole>();
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userName">用户名</param>
        public TwoFactorIdentityUser(string userName):base(userName)
        {
            Roles = new List<TRole>();
        }
        /// <summary>
        /// 用户角色
        /// </summary>
        [BsonIgnore]
        public new List<TRole> Roles { get; set; }
    }
    /// <summary>
    /// 两阶段验证用户定义
    /// </summary>
    public class TwoFactorIdentityUser: MongodbIdentityUser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TwoFactorIdentityUser() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userName">用户名</param>
        public TwoFactorIdentityUser(string userName) :base(userName) { }
        /// <summary>
        /// 邮件
        /// </summary>
       // public string Email { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 邮件确认是否启用
        /// </summary>
       // public bool EmailConfirmed { get; set; }
        /// <summary>
        /// 两阶段提交是否启用
        /// </summary>
        public bool EnabledTwoFactor { get; set; }
        public override void Fill(IEntity entity)
        {
            base.Fill(entity);
            var newEntity = entity as TwoFactorIdentityUser;
            if (newEntity != null)
            {
                Email = newEntity.Email;
                Mobile = newEntity.Mobile;
                EmailConfirmed = newEntity.EmailConfirmed;
                EnabledTwoFactor = newEntity.EnabledTwoFactor;
            }
        }
    }
}