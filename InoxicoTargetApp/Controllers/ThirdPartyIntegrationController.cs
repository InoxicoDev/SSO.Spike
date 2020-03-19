using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace InoxicoTargetApp.Controllers
{
    public class ThirdPartyIntegrationController : Controller
    {
        private const string InoxicoStsAuthAddress = "https://localhost:44301/connect/authorize";
        private const string ThirdPartyStsBaseAddress = "https://localhost:44303";
        private static readonly HttpClient _httpClient = new HttpClient();

        [HttpPost]
        public async Task<string> AuthenticateExternalUser(string clientId)
        {
            try
            {
                var idToken = this.Request.Headers["id"];

                if (string.IsNullOrEmpty(idToken))
                {
                    throw new Exception("No ID Token supplied");
                }

                var user = await ValidateToken(idToken);
                if (user == null)
                {
                    throw new Exception("Invalid token");
                }

                var externalUserId = user.Claims.Single(p => p.Type == "sub").Value;

                var requestUrl = new RequestUrl(InoxicoStsAuthAddress);
                var redirectUrl = requestUrl.CreateAuthorizeUrl(
                    clientId: "inox_login",
                    responseType: "id_token token",
                    scope: "openid profile read write offline_access",
                    redirectUri: "https://localhost:44302/IntendedLocation",
                    state: "this_is_my_state",
                    nonce: Guid.NewGuid().ToString("N"),
                    acrValues: "idp:ThirdParty");

                return redirectUrl;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        private async Task<ClaimsPrincipal> ValidateToken(string token)
        {
            var url = $"{ThirdPartyStsBaseAddress}/.well-known/openid-configuration";
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(url, new OpenIdConnectConfigurationRetriever(), new HttpDocumentRetriever { RequireHttps = false /* BAD FOR PROD! */ });
            OpenIdConnectConfiguration openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);

            TokenValidationParameters validationParameters =
                new TokenValidationParameters
                {
                    ValidIssuer = ThirdPartyStsBaseAddress,
                    ValidAudiences = new[] { "third_party_client" },
                    IssuerSigningKeys = openIdConfig.SigningKeys
                };

            SecurityToken validatedToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            try
            {
                var user = handler.ValidateToken(token, validationParameters, out validatedToken);
                return user;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}