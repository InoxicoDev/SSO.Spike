using IdentityServer3.Core.Configuration;
using InoxicoIdentity.App_Start;
using InoxicoIdentity.Config;
using Owin;
using Unity;

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

            factory.Register(new Registration<RefCoreRegistry>(r => UnityConfig.GetConfiguredContainer().Resolve<RefCoreRegistry>()));

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
