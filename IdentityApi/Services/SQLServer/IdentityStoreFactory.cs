using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace IdentityApi.Services.SQLServer;

public class IdentityStoreFactory : IDesignTimeDbContextFactory<IdentityStore>
{
    public IdentityStore CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        DbContextOptionsBuilder<IdentityStore> builder = new();

        var connectionString = configuration.GetConnectionString("IdentityStoreConnectionString");

        builder.UseSqlServer(connectionString);

        return new IdentityStore(builder.Options);
    }
}
