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
                    ClientName = "Inoxico Login Client",
                    ClientId = "inox_login",
                    Flow = Flows.Implicit,

                    RedirectUris = new List<string>
                    {
                        "https://localhost:44302/IntendedLocation"
                    },

                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://localhost:44302/IntendedLocation"
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
                    AllowClientCredentialsOnly = false,
                    RequireConsent = false,
                    AllowRememberConsent = false,

                    AllowedCorsOrigins = new List<string>{
                        "https://localhost:44302"
                    },

                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = 3600,
                }
                /*new Client
                {
                    ClientName = "External Auth Code Flow Client",
                    ClientId = "external_authcode",
                    Flow = Flows.AuthorizationCode,

                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    RequireConsent = false,
                    AllowRememberConsent = false,

                    RedirectUris = new List<string>
                    {
                        "https://localhost:44302/IntendedLocation",
                    },

                    ClientUri = "https://identityserver.io",

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
                    AllowClientCredentialsOnly = false,

                    AccessTokenType = AccessTokenType.Jwt
                },*/
            };
        }
    }
}
