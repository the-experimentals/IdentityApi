using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityApi.StartupTasks;

public static class StartupTaskWebHostExtensions
{
    public static async Task RunWithTasksAsync(this IHost webHost, CancellationToken cancellationToken = default)
    {
        // Load all tasks from DI
        var startupTasks = webHost.Services.GetServices<IStartupTask>();

        // Execute all the tasks
        foreach (var startupTask in startupTasks)
        {
            await startupTask.ExecuteAsync(cancellationToken);
        }

        // Start the tasks as normal
        await webHost.RunAsync(cancellationToken);
    }
}
