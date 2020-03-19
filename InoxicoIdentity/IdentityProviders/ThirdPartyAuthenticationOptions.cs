using Microsoft.Owin.Security;

namespace InoxicoIdentity.IdentityProviders
{
    public class ThirdPartyAuthenticationOptions : AuthenticationOptions
    {
        public ThirdPartyAuthenticationOptions() : base("ThirdParty")
        {
        }

        public string SignInAsAuthenticationType { get; set; }

        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }

        public IThirdPartyAuthenticationProvider Provider { get; set; }
    }
}