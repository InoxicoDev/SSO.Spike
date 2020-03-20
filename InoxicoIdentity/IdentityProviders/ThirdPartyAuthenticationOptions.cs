using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;

namespace InoxicoIdentity.IdentityProviders
{
    public class ThirdPartyAuthenticationOptions : AuthenticationOptions
    {
        public ThirdPartyAuthenticationOptions(string signInAsAuthenticationType) : base("ThirdParty")
        {
            AuthenticationMode = AuthenticationMode.Passive;
            CallbackPath = new PathString("/third-party");
            SignInAsAuthenticationType = signInAsAuthenticationType;
        }

        public string SignInAsAuthenticationType { get; set; }

        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }

        public IThirdPartyAuthenticationProvider Provider { get; set; }

        private List<ThirdPartyEntry> _thirdParties = new List<ThirdPartyEntry>();
        public List<ThirdPartyEntry> ThirdParties
        {
            get { return _thirdParties; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _thirdParties = value;
            }
        }
        
        /// <summary>
        /// Id Provider Url Path to finalize processing authentication
        /// </summary>
        public PathString CallbackPath { get; set; }
    }

    public class ThirdPartyEntry
    {
        public ThirdPartyEntry(string clientId, string addressSTS, string issuer, string audience)
        {
            ClientId = clientId;
            AddressSTS = addressSTS;
            Issuer = issuer;
            Audience = audience;
        }

        public string ClientId { get; }
        public string AddressSTS { get; }
        public string Issuer { get; }
        public string Audience { get; }
    }
}