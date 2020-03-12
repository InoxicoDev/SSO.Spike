
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

[assembly: OwinStartup(typeof(InoxicoCoreSample.Startup))]

namespace InoxicoCoreSample
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap = new Dictionary<string, string>();

            appBuilder.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = "Cookies"
            });

            appBuilder.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationType = "TempState",
                AuthenticationMode = AuthenticationMode.Passive
            });
        }
    }
}