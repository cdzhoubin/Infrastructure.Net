using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using Zhoubin.Infrastructure.Common.Tools;

namespace Zhoubin.Infrastructure.Common.Identity.MongoDb
{
    /// <summary>
    /// 用户存储实现
    /// </summary>
    /// <typeparam name="TUser">用户类型</typeparam>
    public class UserTwoFactorStore<TUser> : UserTwoFactorStore<TUser, MongodbIdentityRole>
        where TUser : TwoFactorIdentityUser, new()
    {
    }
    /// <summary>
    /// 两阶段验证启用扩展实现
    /// </summary>
    /// <typeparam name="TUser">用户类型</typeparam>
    /// <typeparam name="TRole">角色类型</typeparam>
    public class UserTwoFactorStore<TUser, TRole> : UserStore<TUser, TRole>, IUserTwoFactorStore<TUser>,
        IUserEmailStore<TUser>
        where TUser : TwoFactorIdentityUser, new()
        where TRole:MongodbIdentityRole,new()
    {
        /// <summary>
        /// 获取用户两阶段验证设置，默认表示不启用
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns>返回两阶段验证设置</returns>
        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EnabledTwoFactor);
        }

        
        /// <summary>
        /// 设置指定用户两阶段提交
        /// </summary>
        /// <param name="user">待设置用户</param>
        /// <param name="enabled">两阶段验证设置</param>
        /// <returns>返回完事设置</returns>
        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() =>
            {
                user.EnabledTwoFactor = enabled;
            });
        }
        /// <summary>
        /// 设置邮件地址
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="email">邮件</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        //public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        //{
        //    return TaskHelper.RunTask(() =>
        //    {
        //        user.Email = email;
        //    });
        //}
        /// <summary>
        /// 获取用户邮件地址
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        //public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        //{
        //    return Task.FromResult(user.Email);
        //}
        
        /// <summary>
        /// 获取邮件确认设置
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        //public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        //{
        //    return Task.FromResult(user.EmailConfirmed);
        //}
        /// <summary>
        /// 发送确认邮件
        /// </summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        //public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        //{
        //    return TaskHelper.RunTask(() =>
        //    {
        //        user.EmailConfirmed = confirmed;
        //    });
        //}
        /// <summary>
        /// 根据邮箱查找用户
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns>返回查询结果</returns>
        //public Task<TUser> FindByEmailAsync(string email, CancellationToken cancellationToken)
        //{
        //    if (string.IsNullOrWhiteSpace(email))
        //    {
        //        throw new ArgumentNullException("email");
        //    }
        //    return TaskHelper.RunTask(() => ObjectStorage.FindOneByCondition<TUser>(new Dictionary<string, object> { { "Email", email } }));
        //}


        //public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        //{
        //    return Task.FromResult(user.NormalizedEmail);
        //}

        //public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        //{
        //    return TaskHelper.RunTask(() =>
        //    {
        //        user.NormalizedEmail = normalizedEmail;
        //    });
        //}
        
    }
}