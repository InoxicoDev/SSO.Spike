using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;
using InoxicoIdentity.App_Start;
using InoxicoIdentity.Config;
using Owin;
using Serilog;

namespace InoxicoIdentity
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            WebApiConfig.Configure(appBuilder);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Trace(outputTemplate: "{Timestamp} [{Level}] ({Name}){NewLine} {Message}{NewLine}{Exception}")
                .CreateLogger();

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
            };

            appBuilder.UseIdentityServer(options);
        }
    }
}
