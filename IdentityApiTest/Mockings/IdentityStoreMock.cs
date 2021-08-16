using System;
using IdentityApi.Services.SQLServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IdentityApiTest.Mockings
{
    public class IdentityStoreMock
    {
        public IdentityStore _store;

        public IdentityStoreMock()
        {
            DbContextOptions<IdentityStore> options = new DbContextOptionsBuilder<IdentityStore>()
                              .UseInMemoryDatabase(databaseName: "testDB")
                              .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                              .Options;

            _store = new IdentityStore(options);
        }
    }
}
