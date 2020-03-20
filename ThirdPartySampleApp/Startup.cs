using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using IdentityModel;
using Common;

namespace ThirdPartySampleApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = OAuth.ThirdPartyAudience,
                ClientSecret = "secret".ToSha256(),
                Authority = Addresses.ThirdPartySTSBase,
                RedirectUri = Addresses.ThirdPartyAppBase + "/",
                ResponseType = "id_token token",
                Scope = "openid email",

                UseTokenLifetime = false,
                SignInAsAuthenticationType = "Cookies",

                SaveTokens = true
            });
        }
    }
}