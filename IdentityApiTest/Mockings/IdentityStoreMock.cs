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

           

            _store.PROFILE.Add(DummyAccountData.defaultProfile);
            _store.PROFILE.Add(DummyAccountData.deleteProfile);

            _store.CREDENTIALS.Add(DummyAccountData.defaultProfilecredential);
            _store.CREDENTIALS.Add(DummyAccountData.deleteProfilecredential);

            _store.REFRESH_TOKENS.Add(DummyAccountData.defaultProfileRefreshToken);
            _store.REFRESH_TOKENS.Add(DummyAccountData.deleteProfileRefreshToken);

            _store.SaveChanges();
        }
    }
}
