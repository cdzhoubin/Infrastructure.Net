using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Zhoubin.Infrastructure.Common.Consul.Discover
{
    public interface IServiceDiscover : IDisposable
    {
        Task<bool> Register(ServiceDescrption serviceDescrption, CancellationToken ct = default(CancellationToken));
        Task<bool> Deregister(string id, CancellationToken ct = default(CancellationToken));

        Task<List<ServiceDescrption>> Get(string name, CancellationToken ct = default(CancellationToken));
        Task<List<ServiceDescrption>> Get(CancellationToken ct = default(CancellationToken));

    }
}
