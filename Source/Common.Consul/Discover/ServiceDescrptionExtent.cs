using Consul;
using System;

namespace Zhoubin.Infrastructure.Common.Consul.Discover
{
    public static class ServiceDescrptionExtent
    {
        
        public static ServiceDescrption GetServiceDescrption(this AgentService service)
        {
            return new ServiceDescrption
            {
                Id = service.ID,
                Name = service.Service,
                Address = service.Address,
                Port = service.Port
            };
        }
        public static AgentServiceRegistration GetAgentServiceRegistration(this ServiceDescrption service)
        {
            AgentServiceCheck check = null;
            if(service.CheckSerivce != null)
            {
                check = new AgentServiceCheck //健康检查
                {
                    DeregisterCriticalServiceAfter = GetTimeSpan(service.CheckSerivce.DeregisterCriticalServiceAfter),//服务启动多久后反注册
                    Interval = GetTimeSpan(service.CheckSerivce.Interval),//健康检查时间间隔，或者称为心跳间隔（定时检查服务是否健康）
                    HTTP = service.CheckSerivce.HTTP,//健康检查地址
                    Timeout = GetTimeSpan(service.CheckSerivce.Timeout)//服务的注册时间
                };
            }
            return new AgentServiceRegistration()
            {
                ID = service.Id,//服务编号保证不重复
                Name = service.Name,//服务的名称
                Address = service.Address,//服务ip地址
                Port = service.Port,//服务端口
                Check = check
            };
        }

        private static int? ParseTimeSpan(TimeSpan? value)
        {
            if (value.HasValue)
            {
                return value.Value.Seconds;
            }

            return default(int?);
        }
        private static TimeSpan? GetTimeSpan(int? second)
        {
            if (second == null || second <= 0)
            {
                return null;
            }

            return TimeSpan.FromSeconds(second.Value);
        }
    }
}
