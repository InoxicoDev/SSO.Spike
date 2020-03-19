using System;
using System.Threading.Tasks;

namespace InoxicoIdentity.IdentityProviders
{
    public interface IThirdPartyAuthenticationProvider
    {
        Task Authenticated(ThirdPartyAuthenticatedContext context);

        Task ReturnEndpoint(ThirdPartyReturnEndpointContext context);
    }
}
