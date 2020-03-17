using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using IdentityModel;

namespace ThirdPartySampleApp
{
    public class Startup
    {
        private const string IdSvrBaseAddress = "https://localhost:44303/";

        public void Configuration(IAppBuilder app)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = "third_party_client",
                ClientSecret = "secret".ToSha256(),
                Authority = IdSvrBaseAddress,
                RedirectUri = "https://localhost:44304/",
                ResponseType = "id_token",
                Scope = "openid email",

                UseTokenLifetime = false,
                SignInAsAuthenticationType = "Cookies",

                SaveTokens = true
            });
        }
    }
}