using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace IdentityApi.Services.SQLServer.Account
{
    public class AccountStoreFactory : IDesignTimeDbContextFactory<AccountStore>
    {
        public AccountStore CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            DbContextOptionsBuilder<AccountStore> builder = new();

            string connectionString = configuration.GetConnectionString("AccountStoreConnectionString");

            builder.UseSqlServer(connectionString);

            return new AccountStore(builder.Options);
        }
    }
}
