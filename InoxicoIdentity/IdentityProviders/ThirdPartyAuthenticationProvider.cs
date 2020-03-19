using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace InoxicoIdentity.IdentityProviders
{
    public class ThirdPartyAuthenticationProvider : IThirdPartyAuthenticationProvider
    {
        public ThirdPartyAuthenticationProvider()
        {
            OnAuthenticated = context => Task.FromResult<object>(null);
            OnReturnEndpoint = context => Task.FromResult<object>(null);
        }

        public Func<ThirdPartyAuthenticatedContext, Task> OnAuthenticated { get; set; }

        public Func<ThirdPartyReturnEndpointContext, Task> OnReturnEndpoint { get; set; }

        public Task Authenticated(ThirdPartyAuthenticatedContext context)
        {
            return OnAuthenticated(context); 
        }

        public Task ReturnEndpoint(ThirdPartyReturnEndpointContext context)
        {
            return OnReturnEndpoint(context);
        }
    }
}