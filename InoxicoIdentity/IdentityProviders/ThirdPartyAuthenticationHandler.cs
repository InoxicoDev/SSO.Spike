using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InoxicoIdentity.IdentityProviders
{
    public class ThirdPartyAuthenticationHandler : AuthenticationHandler<ThirdPartyAuthenticationOptions>
    {
        private const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";

        private readonly ILogger _logger;

        public ThirdPartyAuthenticationHandler(ILogger logger)
        {
            _logger = logger;
        }

        protected override async Task ApplyResponseChallengeAsync()
        {
            if (Response.StatusCode != 401)
            {
                return;
            }

            var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);
            if (challenge == null)
            {
                return;
            }

            var baseUri =
                Request.Scheme +
                Uri.SchemeDelimiter +
                Request.Host +
                Request.PathBase;

            var currentUri =
                baseUri +
                Request.Path +
                Request.QueryString;

            var redirectUri =
                baseUri +
                Options.CallbackPath;

            var properties = challenge.Properties;
            if (string.IsNullOrEmpty(properties.RedirectUri))
            {
                properties.RedirectUri = currentUri;
            }

            // OAuth2 10.12 CSRF
            GenerateCorrelationId(properties);

            var state = Options.StateDataFormat.Protect(properties);

            var authorizationEndpoint = new StringBuilder(redirectUri);
            if (!string.IsNullOrEmpty(state))
            {
                authorizationEndpoint.Append("?state=" + Uri.EscapeDataString(state));
            }

            /*var authorizationEndpoint =
                redirectUri +
                "?client_id=" + Uri.EscapeDataString(Request.Query["client_id"]) +
                "&scope=" + Uri.EscapeDataString(Request.Query["scope"]) +
                "&state=" + Uri.EscapeDataString(state);*/

            Response.Redirect(authorizationEndpoint.ToString());
        }

        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            AuthenticationProperties properties = null;

            try
            {
                string state = null;

                var query = Request.Query;
                var values = query.GetValues("state");
                if (values != null && values.Count == 1)
                {
                    state = values[0];
                }

                properties = Options.StateDataFormat.Unprotect(state);
                if (properties == null)
                {
                    return null;
                }

                var context = new ThirdPartyAuthenticatedContext(Context)
                {
                    Identity = new ClaimsIdentity(
                        Options.AuthenticationType,
                        ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType)
                };
                context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "inoxicoUser1", XmlSchemaString, Options.AuthenticationType));

                context.Properties = new AuthenticationProperties
                {
                    RedirectUri = "https://localhost:44302/IntendedLocation"
                };

                await Options.Provider.Authenticated(context);

                return new AuthenticationTicket(context.Identity, context.Properties);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex.Message);
            }

            return new AuthenticationTicket(null, properties);
        }

        public override async Task<bool> InvokeAsync()
        {
            if (!Options.CallbackPath.HasValue || Options.CallbackPath != Request.Path)
            {
                return false;
            }

            AuthenticationTicket model = await AuthenticateAsync();
            if (model == null)
            {
                _logger.WriteWarning("Invalid return state, unable to redirect.");
                base.Response.StatusCode = 500;
                return true;
            }

            var context = new ThirdPartyReturnEndpointContext(base.Context, model)
            {
                SignInAsAuthenticationType = base.Options.SignInAsAuthenticationType,
                RedirectUri = model.Properties.RedirectUri
            };
            model.Properties.RedirectUri = null;
            await base.Options.Provider.ReturnEndpoint(context);

            if (context.SignInAsAuthenticationType != null && context.Identity != null)
            {
                ClaimsIdentity claimsIdentity = context.Identity;
                if (!string.Equals(claimsIdentity.AuthenticationType, context.SignInAsAuthenticationType, StringComparison.Ordinal))
                {
                    claimsIdentity = new ClaimsIdentity(claimsIdentity.Claims, context.SignInAsAuthenticationType, claimsIdentity.NameClaimType, claimsIdentity.RoleClaimType);
                }
                base.Context.Authentication.SignIn(context.Properties, claimsIdentity);
            }
            if (!context.IsRequestCompleted && context.RedirectUri != null)
            {
                if (context.Identity == null)
                {
                    context.RedirectUri = WebUtilities.AddQueryString(context.RedirectUri, "error", "access_denied");
                }
                base.Response.Redirect(context.RedirectUri);
                context.RequestCompleted();
            }
            return context.IsRequestCompleted;
        }
    }
}