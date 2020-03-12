using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer3.Core;
using IdentityServer3.Core.Services.InMemory;

namespace InoxicoIdentity.Config
{
    class Users
    {
        public static List<InMemoryUser> Get()
        {
            var users = new List<InMemoryUser>
            {
                new InMemoryUser{Subject = "inoxicoUser1", Username = "steve", Password = "steve",
                    Claims = new Claim[]
                    {
                        new Claim(Constants.ClaimTypes.Name, "Steve Doe"),
                        new Claim(Constants.ClaimTypes.GivenName, "Steve"),
                        new Claim(Constants.ClaimTypes.FamilyName, "Doe"),
                        new Claim(Constants.ClaimTypes.Email, "steve@email.com"),
                        new Claim(Constants.ClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    }
                },
            };

            return users;
        }
    }
}
