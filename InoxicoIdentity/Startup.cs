using System;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using InoxicoIdentity.App_Start;
using InoxicoIdentity.Config;
using InoxicoIdentity.IdentityProviders;
using Owin;
using Serilog;

namespace InoxicoIdentity
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            WebApiConfig.Configure(app);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Trace(outputTemplate: "{Timestamp} [{Level}] ({Name}){NewLine} {Message}{NewLine}{Exception}")
                .CreateLogger();

            var factory = new IdentityServerServiceFactory();
            
            factory
                .UseInMemoryClients(Clients.Get())
                .UseInMemoryScopes(Scopes.Get())
                .UseInMemoryUsers(Users.Get());

            factory.UserService = new Registration<IUserService, CustomUserService>();

            var options = new IdentityServerOptions
            {
                SiteName = "Inoxico Sample Identity Server",

                SigningCertificate = Certificate.Get(),
                Factory = factory,
                AuthenticationOptions = new AuthenticationOptions
                {
                    IdentityProviders = ConfigureAdditionalIdentityProviders
                }
            };

            app.UseIdentityServer(options);
        }

        private static void ConfigureAdditionalIdentityProviders(IAppBuilder app, string signInAsType)
        {
            app.UseThirdPartyAuthentication(new ThirdPartyAuthenticationOptions { SignInAsAuthenticationType = signInAsType });
        }
    }
}
