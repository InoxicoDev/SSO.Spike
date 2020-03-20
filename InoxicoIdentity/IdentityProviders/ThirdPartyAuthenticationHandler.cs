using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Collections.Concurrent;
using Microsoft.Owin;
using System.Linq;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Extensions;

namespace InoxicoIdentity.IdentityProviders
{
    public class ThirdPartyAuthenticationHandler : AuthenticationHandler<ThirdPartyAuthenticationOptions>
    {
        private const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";

        private static readonly ConcurrentDictionary<string, StateEntry> AuthStateHolder = new ConcurrentDictionary<string, StateEntry>();

        private readonly ILogger _logger;

        public ThirdPartyAuthenticationHandler(ILogger logger)
        {
            _logger = logger;
        }

        public IUserService UserService
        {
            get
            {
                var userService = base.Context?.Environment?.ResolveDependency<IUserService>();
                if (userService == null)
                {
                    return null;
                }
                return userService;
            }
        }

        /// <summary>
        /// For AuthenticationMode = Passive, this gets invoked first when any request comes in.
        /// The idea is to filter out all requests except the ones that contain our configured
        /// CallbackPath path names (i.e. /third-party) in the incoming URL Request.
        /// This should get invoked by the 3rd party STS when the user is successfully authenticated.
        /// </summary>
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

        /// <summary>
        /// For AuthenticationMode = Active, this gets invoked first when any request comes in.
        /// But its best to keep it Passive for this logic to work.
        /// This needs to go and fetch the identity claims from the 3rd party STS.
        /// </summary>
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

                // OAuth2 10.12 CSRF
                if (!ValidateCorrelationId(properties, _logger))
                {
                    return new AuthenticationTicket(null, properties);
                }

                var context = new ThirdPartyAuthenticatedContext(Context)
                {
                    Identity = new ClaimsIdentity(
                        Options.AuthenticationType,
                        ClaimsIdentity.DefaultNameClaimType,
                        ClaimsIdentity.DefaultRoleClaimType)
                };
                context.Identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "inoxicoUser1", XmlSchemaString, Options.AuthenticationType));

                context.Properties = properties;

                await Options.Provider.Authenticated(context);

                return new AuthenticationTicket(context.Identity, context.Properties);
            }
            catch (Exception ex)
            {
                _logger.WriteError(ex.Message);
            }

            return new AuthenticationTicket(null, properties);
        }

        /// <summary>
        /// This gets invoked last when the request is done.
        /// This is a hookin point for redirecting the user to the 3rd
        /// party STS once it picks up that its a auth challenge that
        /// needs to take place. That in turn should redirect to our
        /// CallbackPath when that STS authentication was successful.
        /// </summary>
        protected override async Task ApplyResponseChallengeAsync()
        {
            if (IsAuthRequestWithLoginRedirect(out string[] location))
            {
                StoreRequestWithLoginRedirectDetails(location);
            }

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

            var visitUri =
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

            var authorizationEndpoint = new StringBuilder(visitUri);
            authorizationEndpoint.Append($"?redirect_uri=https://localhost:44302/IntendedLocation");
            if (!string.IsNullOrEmpty(state))
            {
                authorizationEndpoint.Append("&state=" + Uri.EscapeDataString(state));
            }

            Response.Redirect(authorizationEndpoint.ToString());
        }

        private bool IsAuthRequestWithLoginRedirect(out string[] location)
        {
            location = null;
            return Request.Path.Value == "/connect/authorize" && Response.Headers.TryGetValue("Location", out location);
        }

        private void StoreRequestWithLoginRedirectDetails(string[] location)
        {
            AuthStateHolder.AddOrUpdate(location.First(), new StateEntry(Request.Query), (x, y) => new StateEntry(Request.Query));
            var now = DateTime.Now;
            foreach (var entry in AuthStateHolder.Where(p => p.Value.Expire <= now).ToArray())
            {
                AuthStateHolder.TryRemove(entry.Key, out StateEntry val);
            }
        }

        private IReadableStringCollection GetLoginState()
        {
            if(!AuthStateHolder.TryGetValue(Request.Uri.AbsoluteUri, out StateEntry entry))
            {
                return null;
            }

            if (entry.Expire <= DateTime.Now)
            {
                AuthStateHolder.TryRemove(Request.Uri.AbsoluteUri, out StateEntry val);
                return null;
            }

            return entry.KeyValuePairs;
        }

        private const string ThirdPartyStsBaseAddress = "https://localhost:44303";

        private async Task<ClaimsPrincipal> ValidateToken(string token)
        {
            var url = $"{ThirdPartyStsBaseAddress}/.well-known/openid-configuration";
            var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(url, new OpenIdConnectConfigurationRetriever());
            var openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);

            var validationParameters = new TokenValidationParameters
                {
                    ValidIssuer = ThirdPartyStsBaseAddress,
                    ValidAudiences = new[] { "third_party_client" },
                    IssuerSigningKeys = openIdConfig.SigningKeys
                };

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var user = handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return user;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static (string ClientId, string Token) DecodeState(string encodedState)
        {
            if(encodedState == null)
            {
                return (null, null);
            }

            var decodedString = Encoding.UTF8.GetString(Convert.FromBase64String(encodedState));
            var parts = decodedString.Split(';');

            if (parts.Length != 2)
            {
                return (null, null);
            }

            return (parts[0], parts[1]);
        }

        private class StateEntry
        {
            public StateEntry(IReadableStringCollection keyValuePairs)
            {
                KeyValuePairs = keyValuePairs;
                Expire = DateTime.Now.AddMinutes(5);
            }

            public IReadableStringCollection KeyValuePairs { get; }
            public DateTime Expire { get; private set; }
        }
    }
}