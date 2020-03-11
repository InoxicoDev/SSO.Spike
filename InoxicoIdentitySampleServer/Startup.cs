using IdentityServer3.Core.Configuration;
using InoxicoIdentitySampleServer.Config;
using IdentityServer3.Core.Services;
using InoxicoIdentitySampleServer.Extensions;
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

            factory.ClaimsProvider = new Registration<IClaimsProvider>(typeof(GenericClaimsProvider));
            factory.UserService = new Registration<IUserService>(typeof(GenericUserService));
            factory.CustomGrantValidators.Add(new Registration<ICustomGrantValidator>(typeof(ExternalGrantValidator)));

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
