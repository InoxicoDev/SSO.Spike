﻿using System.Collections.Generic;
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
                    ClientId = "third_party_client",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    Flow = Flows.Implicit,

                    AllowedScopes = new List<string>
                    {
                        Constants.StandardScopes.OpenId,
                        Constants.StandardScopes.Profile,
                        Constants.StandardScopes.Email,
                        Constants.StandardScopes.Roles,
                        Constants.StandardScopes.Address,
                    },

                    ClientUri = "https://identityserver.io",

                    RequireConsent = false,
                    AllowRememberConsent = false,

                    RedirectUris = new List<string>
                    {
                        "https://localhost:44304/"
                    },

                    LogoutUri = "http://localhost:44304/Home/SignoutCleanup",
                    LogoutSessionRequired = true,
                    AccessTokenLifetime = 18000
                },
            };
        }
    }
}
