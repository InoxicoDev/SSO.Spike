using System.Collections.Generic;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;

namespace ThirdPartyIdentity.Config
{
    class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "Third Party Client",
                    Enabled = true,

                    ClientId = "third_party_client",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret1".Sha256()),
                    },

                    Flow = Flows.Implicit,
                    ClientUri = "https://identityserver.io",

                    RequireConsent = true,
                    AllowRememberConsent = true,

                    AllowedScopes = new List<string>
                    {
                        Constants.StandardScopes.OpenId,
                        Constants.StandardScopes.Profile,
                        Constants.StandardScopes.Email,
                        Constants.StandardScopes.Roles,
                        Constants.StandardScopes.Address,
                    },

                    RedirectUris = new List<string>
                    {
                        "http://localhost:57255/"
                    },

                    LogoutUri = "http://localhost:59185/Home/SignoutCleanup",
                    LogoutSessionRequired = true,
                },
            };
        }
    }
}
