using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace InoxicoIdentity.IdentityProviders
{
    public class ThirdPartyAuthenticatedContext : BaseContext
    {
        public ThirdPartyAuthenticatedContext(IOwinContext context) : base(context)
        {
        }

        public ClaimsIdentity Identity { get; set; }

        public AuthenticationProperties Properties { get; set; }
    }
}