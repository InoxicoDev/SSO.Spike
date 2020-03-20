using IdentityServer3.Core.Configuration;
using Owin;
using ThirdPartyIdentity.Config;

namespace ThirdPartyIdentity
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var factory = new IdentityServerServiceFactory();
            factory
                .UseInMemoryClients(Clients.Get())
                .UseInMemoryScopes(Scopes.Get())
                .UseInMemoryUsers(Users.Get());

            var options = new IdentityServerOptions
            {
                SiteName = "Third Party Identity Server",

                SigningCertificate = Certificate.Get(),
                Factory = factory,
                AuthenticationOptions = { EnableLocalLogin = true },
            };

            appBuilder.UseIdentityServer(options);
        }
    }
}