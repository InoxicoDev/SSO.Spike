using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InoxicoIdentity.IdentityProviders
{
    public static class ThirdPartyAuthenticationExtensions
    {
        public static IAppBuilder UseThirdPartyAuthentication(this IAppBuilder app, ThirdPartyAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            app.Use(typeof(ThirdPartyAuthenticationMiddleware), app, options);
            return app;
        }
    }
}