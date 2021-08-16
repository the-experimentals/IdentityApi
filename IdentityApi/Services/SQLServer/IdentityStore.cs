using System;
using Microsoft.EntityFrameworkCore;

namespace IdentityApi.Services.SQLServer
{
    public class IdentityStore : DbContext
    {
        public IdentityStore(DbContextOptions<IdentityStore> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) { }
    }
}
