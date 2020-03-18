using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace InoxicoIdentity.Controllers
{
    public class ReferenceCodeController : ApiController
    {
        private readonly RefCodeRegistry _refCodeRegistry;

        public ReferenceCodeController(RefCodeRegistry refCodeRegistry)
        {
            _refCodeRegistry = refCodeRegistry;
        }

        [HttpPut]
        public async Task<string> Create()
        {
            if(!this.Request.Headers.TryGetValues("id", out IEnumerable<string> idTokens))
            {
                throw new Exception("No ID Token supplied");
            }

            var idToken = idTokens.Single();

            var user = await ValidateToken(idToken);
            if (user == null)
            {
                throw new Exception("Invalid token");
            }

            var externalUserId = user.Claims.Single(p => p.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;
            return _refCodeRegistry.CreateRefCodeForUser(externalUserId);
        }

        private async Task<ClaimsPrincipal> ValidateToken(string token)
        {
            var url = $"https://localhost:44303/.well-known/openid-configuration";
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(url, new OpenIdConnectConfigurationRetriever(), new HttpDocumentRetriever { RequireHttps = false /* BAD FOR PROD! */ });
            OpenIdConnectConfiguration openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);

            TokenValidationParameters validationParameters =
                new TokenValidationParameters
                {
                    ValidIssuer = "https://localhost:44303",
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
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}