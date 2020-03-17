using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using InoxicoIdentity.App_Start;
using InoxicoIdentity.Config;
using InoxicoIdentity.Extensions;
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

            factory.Register(new Registration<RefCodeRegistry>(r => UnityConfig.GetConfiguredContainer().Resolve<RefCodeRegistry>()));
            factory.UserService =
                new Registration<IUserService>(typeof(GenericUserService));
            factory.CustomGrantValidators.Add(
                new Registration<ICustomGrantValidator>(typeof(RefCodeGrantValidator)));

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
