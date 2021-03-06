﻿using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using System;

namespace InoxicoIdentity.IdentityProviders
{
    public class ThirdPartyAuthenticationMiddleware : AuthenticationMiddleware<ThirdPartyAuthenticationOptions>
    {
        private readonly ILogger _logger;

        public ThirdPartyAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, ThirdPartyAuthenticationOptions options)
            : base(next, options)
        {
            _logger = app.CreateLogger<ThirdPartyAuthenticationMiddleware>();

            if (options.ThirdParties.Count == 0)
            {
                throw new Exception("No Third Parties configured for Third Party Authentication");
            }

            if (Options.Provider == null)
            {
                Options.Provider = new ThirdPartyAuthenticationProvider();
            }

            if (Options.StateDataFormat == null)
            {
                var dataProtector = app.CreateDataProtector(
                    typeof(ThirdPartyAuthenticationMiddleware).FullName,
                    Options.AuthenticationType, "v1");
                Options.StateDataFormat = new PropertiesDataFormat(dataProtector);
            }

            if (string.IsNullOrEmpty(Options.SignInAsAuthenticationType))
            {
                Options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();
            }
        }

        protected override AuthenticationHandler<ThirdPartyAuthenticationOptions> CreateHandler()
        {
            return new ThirdPartyAuthenticationHandler(_logger);
        }
    }
}