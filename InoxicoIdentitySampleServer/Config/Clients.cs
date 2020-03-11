using System.Collections.Generic;
using IdentityServer3.Core;
using IdentityServer3.Core.Models;

namespace InoxicoIdentitySampleServer.Config
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

                    ClientUri = "https://identityserver.io",

                    RedirectUris = new List<string>
                    {
                        "https://localhost:44312/callback",
                    },

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
                    EnableLocalLogin = true,
                    AccessTokenLifetime = 3600,

                    AbsoluteRefreshTokenLifetime = 86400,
                    SlidingRefreshTokenLifetime = 43200,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    RefreshTokenExpiration = TokenExpiration.Sliding,

                },
                new Client
                {
                    ClientName = "Third Party Client",
                    Enabled = true,

                    ClientId = "third_party_client",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret1".Sha256()),
                    },

                    Flow = Flows.Custom,
                    AllowedCustomGrantTypes = new List<string>
                    {
                        "third_party_client_grant"
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
