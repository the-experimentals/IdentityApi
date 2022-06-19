using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityApi.StartupTasks.Tasks;

public class WarmupServices : IStartupTask
{
    private readonly IServiceProvider _provider;
    private readonly IServiceCollection _services;

    public WarmupServices(IServiceCollection services, IServiceProvider provider)
    {
        _services = services;
        _provider = provider;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _provider.CreateScope();

        foreach (var singleton in GetServices(_services))
        {
            scope.ServiceProvider.GetServices(singleton);
        }

        return Task.CompletedTask;
    }

    private static IEnumerable<Type> GetServices(IServiceCollection services)
    {
        return services
            .Where(descriptor => descriptor.ImplementationType != typeof(WarmupServices))
            .Where(descriptor => descriptor.ServiceType.ContainsGenericParameters == false)
            .Select(descriptor => descriptor.ServiceType)
            .Distinct();
    }
}
