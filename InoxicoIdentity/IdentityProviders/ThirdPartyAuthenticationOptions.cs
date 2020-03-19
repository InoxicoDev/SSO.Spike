using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace InoxicoIdentity.IdentityProviders
{
    public class ThirdPartyAuthenticationOptions : AuthenticationOptions
    {
        public ThirdPartyAuthenticationOptions() : base("ThirdParty")
        {
            AuthenticationMode = AuthenticationMode.Passive;
            CallbackPath = new PathString("/third-party");
        }

        public string SignInAsAuthenticationType { get; set; }

        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }

        public IThirdPartyAuthenticationProvider Provider { get; set; }

        /// <summary>
        /// Id Provider Url Path to finalize processing authentication
        /// </summary>
        public PathString CallbackPath { get; set; }
    }
}