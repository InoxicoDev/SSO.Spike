using System.Collections.Generic;
using Common;
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
                    ClientId = OAuth.InoxicoClientId,
                    Flow = Flows.Implicit,

                    RedirectUris = new List<string>
                    {
                        Addresses.IntendedLocation
                    },

                    PostLogoutRedirectUris = new List<string>
                    {
                        Addresses.IntendedLocation
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
                        Addresses.InoxicoTargetAppBase
                    },

                    AccessTokenType = AccessTokenType.Jwt,
                    AccessTokenLifetime = 3600,
                }
            };
        }
    }
}
