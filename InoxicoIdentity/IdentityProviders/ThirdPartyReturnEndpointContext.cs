using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InoxicoIdentity.IdentityProviders
{
    public class ThirdPartyReturnEndpointContext : ReturnEndpointContext
    {
        public ThirdPartyReturnEndpointContext(
           IOwinContext context,
           AuthenticationTicket ticket)
           : base(context, ticket)
        {
        }
    }
}