using System;
using System.Threading;
using System.Threading.Tasks;
using IdentityApi.Services.SQLServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityApi.StartupTasks.Tasks
{
    public class DBMigrator : IStartupTask
    {
        private readonly IServiceProvider _serviceProvider;

        public DBMigrator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            // Create a new scope to retrieve scoped services
            using var scope = _serviceProvider.CreateScope();
            // Get the DbContext instance
            var myDbContext = scope.ServiceProvider.GetRequiredService<IdentityStore>();

            //Do the migration 
            await myDbContext.Database.MigrateAsync();
        }
    }
}
