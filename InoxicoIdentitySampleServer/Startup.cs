using IdentityServer3.Core.Configuration;
using InoxicoIdentitySampleServer.Config;
using Owin;

namespace InoxicoIdentitySampleServer
{
    internal class Startup
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
                SiteName = "Inoxico Sample Identity Server",

                SigningCertificate = Certificate.Get(),
                Factory = factory,
                AuthenticationOptions = { EnableLocalLogin = true }
            };

            appBuilder.UseIdentityServer(options);
        }
    }
}
