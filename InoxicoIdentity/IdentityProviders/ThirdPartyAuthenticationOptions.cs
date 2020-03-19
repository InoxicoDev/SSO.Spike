using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace InoxicoIdentity.IdentityProviders
{
    public class ThirdPartyAuthenticationOptions : AuthenticationOptions
    {
        public ThirdPartyAuthenticationOptions() : base("ThirdParty")
        {
            AuthenticationMode = AuthenticationMode.Active;
            CallbackPath = new PathString("/third-party");
        }

        public string SignInAsAuthenticationType { get; set; }

        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }

        public IThirdPartyAuthenticationProvider Provider { get; set; }

        public PathString CallbackPath { get; set; }
    }
}