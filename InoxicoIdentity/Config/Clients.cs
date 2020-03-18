using System.Collections.Generic;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;

namespace InoxicoIdentity.Config
{
    class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "External Ref Code Flow Client",
                    ClientId = "external_ref_code_client",
                    Flow = Flows.Custom,

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    RequireConsent = false,
                    AllowRememberConsent = false,

                    RedirectUris = new List<string>
                    {
                        "https://localhost:44304/",
                    },

                    AllowedCustomGrantTypes = new List<string> { "refcode_grant" },

                    AllowedScopes = new List<string>
                    {
                        Constants.StandardScopes.OpenId,
                        Constants.StandardScopes.Profile,
                        Constants.StandardScopes.Email,
                        Constants.StandardScopes.Roles,
                        Constants.StandardScopes.OfflineAccess,
                        "read",
                        "write"
                    },
                    AllowClientCredentialsOnly = true,
                },
                
            };
        }
    }
}
