using System;
using System.Collections;
using System.Collections.Generic;
using IdentityApi.RequestModels;

namespace IdentityApiTest.Mockings
{
    public class LogInRequestTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            //yield return new object[]
            //{
            //    new LogInRequest()
            //    {
            //        USERNAME = "testuser",
            //        PASSWORD = "testPassword"
            //    }                
            //};

            yield return new object[]
            {
                new LogInRequest()
                {
                    USERNAME = "default",
                    PASSWORD = "defaultTest"
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
