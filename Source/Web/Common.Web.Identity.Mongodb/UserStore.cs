using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using Zhoubin.Infrastructure.Common.Log;
using Zhoubin.Infrastructure.Common.MongoDb;
using Zhoubin.Infrastructure.Common.Tools;

namespace Zhoubin.Infrastructure.Common.Identity.MongoDb
{
    /// <summary>
    /// 用户存储实现
    /// </summary>
    /// <typeparam name="TUser">用户类型</typeparam>
    public class MongodbUserStore<TUser> : UserStore<TUser, MongodbIdentityRole>
        where TUser : MongodbIdentityUser, new()
    {
    }
    /// <summary>
    /// 用户存储实现
    /// </summary>
    /// <typeparam name="TUser">用户类型</typeparam>
    /// <typeparam name="TRole">角色类型</typeparam>
    public class UserStore<TUser, TRole> : //IUserStore<TUser,ObjectId>,

                          IUserClaimStore<TUser>,

                          IUserLoginStore<TUser>,

                          IUserRoleStore<TUser>,

                          IUserPasswordStore<TUser>,
        IUserEmailStore<TUser>,
                          IUserSecurityStampStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserLockoutStore<TUser> where TUser : MongodbIdentityUser, new()
        where TRole : MongodbIdentityRole, new()
    {
        private IObjectStorage _objectStorage;
        /// <summary>
        /// 对象存储
        /// </summary>
        protected IObjectStorage ObjectStorage
        {
            get { return _objectStorage; }
        }

        public IQueryable<TUser> Users {
        get {
                return _objectStorage.FindByQuery<TUser, ObjectId>(p => true, p => p.Id, true).AsQueryable();
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public UserStore()
        {
            _objectStorage = Factory.CreateObjectStorage();
        }

        /// <summary>
        /// 操作运行
        /// </summary>
        /// <param name="action">操作</param>
        /// <typeparam name="T">操作返回类型</typeparam>
        /// <returns>返回操作对象</returns>
        static Task<IdentityResult> RunTask(Action action, string errorCode)
        {
            return TaskHelper.RunTask<IdentityResult>(() =>
            {
                IdentityResult result;
                try
                {
                    action();
                    result = IdentityResult.Success;
                }
                catch (Exception ex)
                {
                    LogFactory.GetDefaultLogger().Write(new LogExceptionEntity(ex));
                    result = IdentityResult.Failed(new IdentityError() { Code = errorCode, Description = ex.Message });
                }
                return result;
            });
        }
        /// <summary>Insert a new user</summary>
        /// <param name="user">用户实体</param>
        /// <returns>创建成功返回</returns>
        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return RunTask(() => user.Id = new ObjectId(_objectStorage.Insert(user)), "操作出现异常");
        }


        /// <summary>Update a user</summary>
        /// <param name="user">用户实体</param>
        /// <returns>更新成功返回</returns>
        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return RunTask(() => _objectStorage.Update(user), "更新用户失败");
        }


        /// <summary>Delete a user</summary>
        /// <param name="user">用户实体</param>
        /// <returns>删除成功返回</returns>
        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            return RunTask(() => _objectStorage.Delete<MongodbIdentityUser>(user.Id.ToString()), "删除用户失败");
            
        }


        /// <summary>Finds a user</summary>
        /// <param name="userId">用户编号</param>
        /// <returns>返回查询到的用户实体</returns>
        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Null or empty argument: userId");
            }

            return TaskHelper.RunTask(() =>
            {
                var result = _objectStorage.FindById<TUser>(userId);


                return result;
            });
        }


        /// <summary>Find a user by name</summary>
        /// <param name="userName">用户名</param>
        /// <returns>返回查询到的用户实体</returns>
        public Task<TUser> FindByNameAsync(string userName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }



            return TaskHelper.RunTask(() =>
            {
                var result = _objectStorage.FindByQuery<TUser, string>(p => p.UserName == userName, p => p.UserName, false);

                // Should I throw if > 1 user?
                if (result != null && result.Count == 1)
                {
                    return result[0];
                }

                return default(TUser);
            });
        }


        /// <summary>执行与释放或重置非托管资源关联的应用程序定义的任务。</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_objectStorage != null)
            {
                _objectStorage.Dispose();
                _objectStorage = null;
            }
        }


        /// <summary>
        ///     Returns the claims for the user with the issuer set
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <returns>返回用户的Claim列表</returns>
        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() =>
            {
                var identity = _objectStorage.FindByQuery<UserClaims,ObjectId>(p=>p.UserId == user.Id,p=>p.Id,true);
                if(identity == null)
                {
                    return new List<Claim>();
                }
                return (IList<Claim>)identity.Select(p=>p.Claim).ToList();
            });
        }
        /// <summary>
        ///     Adds a user login with the specified provider and key
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <param name="login">登录信息</param>
        /// <returns>成功返回</returns>
        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            return TaskHelper.RunTask(() =>
            {
                var userId = user.Id;
                _objectStorage.Insert(new UserLogins
                {
                    ProviderKey = login.ProviderKey,
                    LoginProvider = login.LoginProvider,
                    UserId = userId
                });
            });
        }


        


        /// <summary>Returns the linked accounts for this user</summary>
        /// <param name="user">用户实体</param>
        /// <returns>返回登录后用户信息</returns>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return TaskHelper.RunTask(() =>
            {
                var userId = user.Id.ToString();
                IList<UserLoginInfo> logins = _objectStorage.FindByCondition<UserLogins>(new Dictionary<string, object> { { "UserId", userId } }, new Dictionary<string, bool>()).ConvertAll(p => new UserLoginInfo(p.LoginProvider, p.ProviderKey, ""));

                return logins.Count == 0 ? null : logins;
            });
        }


        /// <summary>Returns the user associated with this login</summary>
        /// <param name="login">用户登录信息</param>
        /// <returns>返回查询到的用户</returns>
        public Task<TUser> FindAsync(UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }


            return TaskHelper.RunTask(() =>
            {
                var userId = _objectStorage.FindOneByCondition<UserLogins>(p => p.LoginProvider == login.LoginProvider && p.ProviderKey == login.ProviderKey).UserId;
                if (userId != ObjectId.Empty)
                {
                    var user = _objectStorage.FindById<TUser>(userId.ToString());
                    if (user != null)
                    {
                        return user;
                    }
                }

                return default(TUser);
            });
        }


        /// <summary>Adds a user to a role</summary>
        /// <param name="user">用户实体</param>
        /// <param name="roleName">角色名称</param>
        /// <returns>返回添加结果</returns>
        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Argument cannot be null or empty: roleName.");
            }



            return TaskHelper.RunTask(() =>
            {
                var roleEntity = _objectStorage.FindOneByCondition<TRole>(p => p.Name == roleName);
                if (roleEntity != null)
                {
                    AddUserRole(user.Id, user.UserName, roleEntity.Id, roleEntity.Name);
                }
            });
        }

        private void AddUserRole(ObjectId userId, string userName, ObjectId roleId, string roleName)
        {
            _objectStorage.Insert(new UserRole { RoleName = roleName, RoleId = roleId, UserId = userId, UserName = userName });
        }


        /// <summary>Removes the role for the user</summary>
        /// <param name="user">用户实体</param>
        /// <param name="roleName">角色名</param>
        /// <returns>出错，抛出异常</returns>
        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Argument cannot be null or empty: roleName.");
            }


            return TaskHelper.RunTask(() =>
            {
                _objectStorage.Delete<UserRole>(p => p.RoleName == roleName && p.UserName == user.UserName);
            });
        }


        /// <summary>Returns the roles for this user</summary>
        /// <param name="user">用户实体</param>
        /// <returns>返回用户角色列表</returns>
        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            var userId = user.Id;
            return TaskHelper.RunTask(() =>
            {
                IList<string> roles = _objectStorage.FindByQuery<UserRole, string>(p => p.UserId == userId, p => p.RoleName, true).ConvertAll(p => p.RoleName);
                return roles;
            });
        }


        /// <summary>Returns true if a user is in the role</summary>
        /// <param name="user">用户实体</param>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("roleName");
            }


            return TaskHelper.RunTask(() =>
            {
                var userId = user.Id;
                return _objectStorage.Any<UserRole>(p => p.UserId == userId && p.RoleName == roleName);

            });
        }


        /// <summary>Set the user password hash</summary>
        /// <param name="user">用户实体</param>
        /// <param name="passwordHash">密码hash值</param>
        /// <returns>返回结果</returns>
        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => { user.PasswordHash = passwordHash; });
        }       


        /// <summary>Get the user password hash</summary>
        /// <param name="user">用户实体</param>
        /// <returns>获取用户密码hash</returns>
        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => user.PasswordHash);
        }


        /// <summary>Returns true if a user has a password set</summary>
        /// <param name="user">用户实体</param>
        /// <returns>用户已经设置密码返回true,其他返回false</returns>
        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => !string.IsNullOrEmpty(user.PasswordHash));
        }

        /// <summary>
        /// 设置安全字符串
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="stamp">密码</param>
        /// <returns></returns>
        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => { user.SecurityStamp = stamp; });
        }

        /// <summary>
        /// 获取安全字符串
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns>返回字符串</returns>
        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => user.SecurityStamp);
        }
        /// <summary>
        /// 获取锁定时间
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>返回锁定时间</returns>
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => user.LockoutEndDate);
        }
        /// <summary>
        /// 设置锁定日期
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <param name="lockoutEnd">设置锁定时间</param>
        /// <returns>返回结果</returns>
        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => user.LockoutEndDate = lockoutEnd.Value);
        }
        /// <summary>
        /// 增加登录失败次数
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>返回失败次数</returns>
        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => user.LoginFail += 1);
        }
        /// <summary>
        /// 重置登录失败次数
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>返回结果</returns>
        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => user.LoginFail = 0);
        }
        /// <summary>
        /// 获取帐户出错次数
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>返回出错次数</returns>
        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => user.LoginFail);
        }


        /// <summary>Add a new user claim</summary>
        /// <param name="user">用户实体</param>
        /// <param name="claims">Claim</param>
        /// <returns>成功直接返回</returns>
        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claims == null)
            {
                throw new ArgumentNullException("claim");
            }
            return TaskHelper.RunTask(() =>
            {
                _objectStorage.Delete<UserClaims>(p => p.UserId == user.Id);
                foreach(var c in claims)
                {
                    _objectStorage.Insert(new UserClaims { UserId = user.Id, Claim = c });
                }
            });
        }


        /// <summary>Remove a user claim</summary>
        /// <param name="user">用户实体</param>
        /// <param name="claims">Claim</param>
        /// <returns>成功返回</returns>
        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {

            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claims == null)
            {
                throw new ArgumentNullException("claim");
            }

            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            return TaskHelper.RunTask(() =>
            {
                _objectStorage.Delete<UserClaims>(p => p.UserId == user.Id && claims.Contains(p.Claim));               

            });
        }

        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null || newClaim == null)
            {
                throw new ArgumentNullException("claim");
            }

            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            return TaskHelper.RunTask(() =>
            {
                var userId = user.Id;
                var result = _objectStorage.FindOneByCondition<UserClaims>(p => p.UserId == user.Id && p.Claim == claim);

                if (result != null)
                {
                    result.Claim = newClaim;
                    _objectStorage.Update(result);
                }
                else
                {
                    result = new UserClaims();
                    result.UserId = user.Id;
                    result.Claim = newClaim;
                    _objectStorage.Insert(result);
                }
            });
        }

        //old
        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() =>
            {
                var result = _objectStorage.FindByQuery<UserClaims,ObjectId>(p => p.Claim == claim,p=>p.Id,true);

                if (result == null || result.Count == 0)
                {
                    return default(IList<TUser>);
                }
                else
                {
                    List<TUser> list = new List<TUser>();
                    result.ForEach(p =>
                    list.Add(_objectStorage.FindById<TUser>(p.UserId.ToString())));

                    return list;
                }
            });
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask<string>(() => user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask<string>(() => user.UserName);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(()=> {
                user.UserName = userName;
            });
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask<string>(() => user.NormalizedUserName);
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => {
                user.NormalizedUserName = normalizedName;
            });
        }


        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(loginProvider))
            {
                throw new ArgumentNullException("loginProvider");
            }
            if (string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentNullException("providerKey");
            }
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly


            return TaskHelper.RunTask(() =>
            {
                var userId = user.Id;
                _objectStorage.Delete<UserLogins>(p => p.UserId == userId && p.LoginProvider == loginProvider && p.ProviderKey == providerKey);
            });
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(loginProvider))
            {
                throw new ArgumentNullException("loginProvider");
            }
            if (string.IsNullOrEmpty(providerKey))
            {
                throw new ArgumentNullException("providerKey");
            }
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly


            return TaskHelper.RunTask(() =>
            {
               var resutl = _objectStorage.FindByQuery<UserLogins,ObjectId>(p => p.LoginProvider == loginProvider && p.ProviderKey == providerKey
                ,p=>p.UserId,true);
                if(resutl == null || resutl.Count == 0)
                {
                    return default(TUser);
                }
                var id = resutl.First().UserId;
                return _objectStorage.FindById<TUser>(id.ToString());
            });
        }

        public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("roleName");
            }
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly


            return TaskHelper.RunTask(() =>
            {
                var resutl = _objectStorage.FindOneByCondition<TRole>(p => p.Name == roleName);
                if (resutl == null)
                {
                    return new List<TUser>();
                }
               var userIds = _objectStorage.FindByQuery<UserRole,ObjectId>(p => p.RoleId == resutl.Id,p=>p.UserId,true);
                List<TUser> list = new List<TUser>();
                userIds.ForEach(p => list.Add(_objectStorage.FindById<TUser>(p.UserId.ToString())));
                return (IList<TUser>)list;
            });
        }


        /// <summary>
        /// 获取帐户锁定状态
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <returns>返回结果</returns>
        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => user.LockoutEndDate > DateTimeOffset.UtcNow);
        }
        /// <summary>
        /// 设置是否锁定
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="enabled">是否锁定</param>
        /// <returns>返回设置结果</returns>
        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => user.LockoutEndDate = DateTimeOffset.MinValue);
        }

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(()=>user.Email = email);
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => user.EmailConfirmed = true);
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(
                () => _objectStorage.FindByQuery<TUser, ObjectId>
                (p => p.NormalizedEmail == normalizedEmail, p => p.Id, false).FirstOrDefault()
                );
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(() => user.NormalizedEmail = normalizedEmail);
        }
    }
}
