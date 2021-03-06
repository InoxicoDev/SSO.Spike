﻿using System;
using System.Collections.Generic;
using Common;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using InoxicoIdentity.App_Start;
using InoxicoIdentity.Config;
using InoxicoIdentity.IdentityProviders;
using InoxicoIdentity.Services;
using Owin;
using Serilog;

namespace InoxicoIdentity
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            WebApiConfig.Configure(app);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Trace(outputTemplate: "{Timestamp} [{Level}] ({Name}){NewLine} {Message}{NewLine}{Exception}")
                .CreateLogger();

            var factory = new IdentityServerServiceFactory();
            
            factory
                .UseInMemoryClients(Clients.Get())
                .UseInMemoryScopes(Scopes.Get())
                .UseInMemoryUsers(Users.Get());

            factory.UserService = new Registration<IUserService, CustomUserService>();

            var options = new IdentityServerOptions
            {
                SiteName = "Inoxico Identity Server",

                SigningCertificate = Certificate.Get(),
                Factory = factory,
                AuthenticationOptions = new AuthenticationOptions
                {
                    IdentityProviders = ConfigureAdditionalIdentityProviders
                }
            };

            app.UseIdentityServer(options);
        }

        private static void ConfigureAdditionalIdentityProviders(IAppBuilder app, string signInAsType)
        {
            app.UseThirdPartyAuthentication(new ThirdPartyAuthenticationOptions(signInAsType)
            {
                ThirdParties = new List<ThirdPartyEntry>
                {
                    new ThirdPartyEntry(Identifiers.ThirdPartyClientId, Addresses.ThirdPartySTSBase, Addresses.ThirdPartySTSBase, OAuth.ThirdPartyAudience)
                }
            });
        }
    }
}
