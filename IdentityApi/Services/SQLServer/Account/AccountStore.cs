using System;
using Microsoft.EntityFrameworkCore;

namespace IdentityApi.Services.SQLServer.Account
{
    public class AccountStore :DbContext
    {
        public AccountStore(DbContextOptions<AccountStore> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder) { }
    }
}
