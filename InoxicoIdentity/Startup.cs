using IdentityServer3.Core.Configuration;
using InoxicoIdentity.App_Start;
using InoxicoIdentity.Config;
using Owin;

namespace InoxicoIdentity
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            WebApiConfig.Configure(appBuilder);

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
                //AuthenticationOptions = { EnableLocalLogin = true }
            };

            appBuilder.UseIdentityServer(options);
        }
    }
}
