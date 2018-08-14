using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Class that implements the key ASP.NET Identity role store iterfaces
    /// </summary>
    public class MongodbRoleStore<TRole> : IRoleStore<TRole>, IQueryableRoleStore<TRole>
        where TRole : MongodbIdentityRole, new()
    {
        private IObjectStorage _objectStorage;

        public IQueryable<TRole> Roles {
        get{
return _objectStorage.FindByQuery<TRole,ObjectId>(p=>true,p=>p.Id,true).AsQueryable();
            }
        }


        /// <summary>
        /// Default constructor that initializes a new ObjectStorage
        /// instance using the Default Connection string
        /// </summary>
        public MongodbRoleStore()
        {
            _objectStorage = Factory.CreateObjectStorage();
        }


        /// <summary>Create a new role</summary>
        /// <param name="role">角色实体</param>
        /// <returns>返回创建结果</returns>
        public Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            return RunTask(() =>
            {
                _objectStorage.Insert(role);
            }, "新增角色");
        }


        /// <summary>Delete a role</summary>
        /// <param name="role">角色实体</param>
        /// <returns>返回删除结果</returns>
        public Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            return RunTask(() =>
            {
                var roleId = role.Id;
                _objectStorage.Delete<TRole>(roleId.ToString());
                _objectStorage.Delete<UserRole>(p => p.RoleId == roleId);
            }, "删除角色");
        }


        /// <summary>Find a role by id</summary>
        /// <param name="roleId">角色编号</param>
        /// <returns>返回角色</returns>
        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(()=>_objectStorage.FindById<TRole>(roleId));
        }



        /// <summary>
        /// 操作运行
        /// </summary>
        /// <param name="action">操作</param>
        /// <typeparam name="T">操作返回类型</typeparam>
        /// <returns>返回操作对象</returns>
        static Task<IdentityResult> RunTask(Action action,string errorCode)
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

        /// <summary>异步更新一个角色</summary>
        /// <param name="role"></param>
        /// <returns>更新任务</returns>
        public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            return RunTask(() =>
            {
                _objectStorage.Update(role);
                var roleId = role.Id;
                var userRoles = _objectStorage.FindByQuery<UserRole, ObjectId>(p => p.RoleId == roleId, p => p.Id, true);
                if (userRoles != null && userRoles.Count > 0)
                {
                    userRoles.ForEach(p =>
                    {
                        p.RoleName = role.Name;
                        _objectStorage.Update(p);
                    });
                }

            },"更新角色");
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


        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            return Task.Run(() => { role.Name = roleName; });
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.Run(() => { role.NormalizedName = normalizedName; });
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return TaskHelper.RunTask(()=>
            _objectStorage.FindOneByCondition<TRole>(p => p.NormalizedName == normalizedRoleName));
        }
    }
}
