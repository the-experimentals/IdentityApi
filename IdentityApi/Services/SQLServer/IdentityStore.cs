using IdentityApi.DataModels;
using Microsoft.EntityFrameworkCore;

namespace IdentityApi.Services.SQLServer;

public class IdentityStore : DbContext
{
    public IdentityStore(DbContextOptions<IdentityStore> options) : base(options) { }

    public DbSet<Credential> CREDENTIALS { get; set; }
    public DbSet<Person> PERSON { get; set; }
    public DbSet<Profile> PROFILE { get; set; }
    public DbSet<RefreshToken> REFRESH_TOKENS { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) { }
}
