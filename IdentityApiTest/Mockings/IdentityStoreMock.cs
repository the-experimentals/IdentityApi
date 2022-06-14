using System;
using IdentityApi.Account;
using IdentityApi.DataModels;
using IdentityApi.Services.SQLServer;
using IdentityApi.Utilities;
using IdentityApiTest.Data;
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
                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                              .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                              .Options;

            _store = new IdentityStore(options);

           

            _store.PROFILE.Add(DummyAccountData.profile);

            _store.CREDENTIALS.Add(DummyAccountData.credential);

            _store.SaveChanges();
        }
    }
}
