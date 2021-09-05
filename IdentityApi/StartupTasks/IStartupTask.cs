using System;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityApi.StartupTasks
{
    public interface IStartupTask
    {
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
