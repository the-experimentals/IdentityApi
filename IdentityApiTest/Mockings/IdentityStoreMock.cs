using System;
using IdentityApi.Services.SQLServer;
using IdentityApiTest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace IdentityApiTest.Mockings;

public class IdentityStoreMock
{
    public IdentityStore _store;

    public IdentityStoreMock()
    {
        var options = new DbContextOptionsBuilder<IdentityStore>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _store = new IdentityStore(options);


        _store.PROFILE.Add(DummyData.systemProfile);
        _store.PROFILE.Add(DummyData.defaultProfile);
        _store.PROFILE.Add(DummyData.deleteProfile);
        _store.PROFILE.Add(DummyData.cacheProfile);
        _store.PROFILE.Add(DummyData.ericProfile);
        _store.PROFILE.Add(DummyData.amyProfile);


        _store.CREDENTIALS.Add(DummyData.systemProfilecredential);
        _store.CREDENTIALS.Add(DummyData.defaultProfilecredential);
        _store.CREDENTIALS.Add(DummyData.deleteProfilecredential);
        _store.CREDENTIALS.Add(DummyData.cacheProfilecredential);
        _store.CREDENTIALS.Add(DummyData.ericProfilecredential);
        _store.CREDENTIALS.Add(DummyData.amyProfilecredential);

        _store.REFRESH_TOKENS.Add(DummyData.systemProfileRefreshToken);
        _store.REFRESH_TOKENS.Add(DummyData.defaultProfileRefreshToken);
        _store.REFRESH_TOKENS.Add(DummyData.deleteProfileRefreshToken);
        _store.REFRESH_TOKENS.Add(DummyData.cacheProfileRefreshToken);
        _store.REFRESH_TOKENS.Add(DummyData.ericProfileRefreshToken);
        _store.REFRESH_TOKENS.Add(DummyData.amyProfileRefreshToken);

        _store.SaveChanges();
    }
}
