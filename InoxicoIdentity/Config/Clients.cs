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
                    ClientName = "Code Flow Client Demo",
                    ClientId = "codeclient",
                    Flow = Flows.AuthorizationCode,

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

                    AccessTokenType = AccessTokenType.Reference,
                    AllowClientCredentialsOnly = true,
                },
                new Client
                {
                    ClientName = "External Client",
                    Enabled = true,

                    ClientId = "external_client",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret1".Sha256()),
                    },

                    Flow = Flows.Custom,
                    AllowedCustomGrantTypes = new List<string>
                    {
                        "refcode_grant"
                    },

                    AllowedScopes = new List<string>
                    {
                        "read",
                        "write",
                    },
                },

            };
        }
    }
}
