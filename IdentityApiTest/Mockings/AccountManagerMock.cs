using System;
using IdentityApi.Account;
using IdentityApi.DataModels;
using IdentityApi.Services.SQLServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;

namespace IdentityApiTest.Mockings
{
    public class AccountManagerMock : IDisposable
    {
        //private IdentityStore _store;
        public IAccountManager _accountManager { get; private set; }

        public AccountManagerMock()
        {
            Mock <IAccountManager> accountManagerMock = new();

            accountManagerMock.Setup(manager => manager.CreateProfile(It.Is<Profile>(p => p.ID == Profile.ADMIN_GUID))).Returns(new ProfileSaveStatus
            {
                IS_SAVED = true
            });

            _accountManager = accountManagerMock.Object;
            
        }

        public void Dispose()
        {
            _accountManager = null;
        }
    }
}
