using Consul;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Zhoubin.Infrastructure.Common.Consul.Discover
{
    public class ConsulServiceDiscover : IServiceDiscover
    {
        IConsulClient _client;
        public ConsulServiceDiscover(IConsulClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            _client = client;
        }
        public async Task<bool> Deregister(string id, CancellationToken ct = default(CancellationToken))
        {
            var result = await _client.Agent.ServiceDeregister(id, ct);
            return await Task.FromResult(CheckStatusCode(result.StatusCode));
        }

        public void Dispose()
        {
        }

        public async Task<List<ServiceDescrption>> Get(string name, CancellationToken ct = default(CancellationToken))
        {
            var result = await _client.Agent.Services(ct);
            if (CheckStatusCode(result.StatusCode))
            {
                List<ServiceDescrption> list = new List<ServiceDescrption>();
                foreach (var service in result.Response.Values.Where(p => p.Service == name))
                {
                    list.Add(service.GetServiceDescrption());
                }
                return await Task.FromResult(list);

            }

            return await Task.FromResult(default(List<ServiceDescrption>));
        }

        public async Task<List<ServiceDescrption>> Get(CancellationToken ct = default(CancellationToken))
        {
            var result = await _client.Agent.Services(ct);
            if (CheckStatusCode(result.StatusCode))
            {
                List<ServiceDescrption> dic = new List<ServiceDescrption>();
                foreach (var key in result.Response.Keys)
                {
                    dic.Add(result.Response[key].GetServiceDescrption());
                }
                return await Task.FromResult(dic);

            }

            return await Task.FromResult(default(List<ServiceDescrption>));
        }

        public async Task<bool> Register(ServiceDescrption serviceDescrption, CancellationToken ct = default(CancellationToken))
        {
            if (serviceDescrption == null)
            {
                throw new ArgumentNullException(nameof(serviceDescrption));
            }
            if (string.IsNullOrEmpty(serviceDescrption.Id))
            {
                serviceDescrption.Id = Guid.NewGuid().ToString().Replace("-", "");
            }
            var result = await _client.Agent.ServiceRegister(serviceDescrption.GetAgentServiceRegistration(), ct);

            return await Task.FromResult(CheckStatusCode(result.StatusCode));
        }

        private bool CheckStatusCode(System.Net.HttpStatusCode httpStatusCode)
        {
            return httpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
