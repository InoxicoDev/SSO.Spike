using IdentityModel.Jwk;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Validation;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InoxicoIdentitySampleServer.Extensions
{
    public class ExternalGrantValidator : ICustomGrantValidator
    {
        public string GrantType => "external_client_grant";

        private readonly IUserService _users;

        public ExternalGrantValidator(IUserService users)
        {
            _users = users;
        }

        public async Task<CustomGrantValidationResult> ValidateAsync(ValidatedTokenRequest request)
        {
            var accessToken = request.Raw.Get("access_token");

            var result = await ValidateToken(accessToken);
            if (result != null)
            {
                return result;
            }

            var jwt = ReconstructJWT(accessToken);

            var context = new ProfileDataRequestContext(new ClaimsPrincipal(new ClaimsIdentity(jwt.ToClaims())), request.Client, "validator");
            await _users.GetProfileDataAsync(context);

            if (context.IssuedClaims == null)
            {
                return new CustomGrantValidationResult("Invalid user");
            }

            return new CustomGrantValidationResult(context.Subject.Claims.First(p => p.Type == "sub").Value, "external", context.IssuedClaims);
        }

        private async Task<CustomGrantValidationResult> ValidateToken(string accessToken)
        {
            var url = $"{ConfigurationManager.AppSettings["ExternalSTS.Domain"]}/.well-known/openid-configuration";
            IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(url, new OpenIdConnectConfigurationRetriever(), new HttpDocumentRetriever { RequireHttps = false /* BAD FOR PROD! */ });
            OpenIdConnectConfiguration openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);

            TokenValidationParameters validationParameters =
                new TokenValidationParameters
                {
                    ValidIssuer = ConfigurationManager.AppSettings["ExternalSTS.Domain"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ExternalSTS.Audience"] },
                    IssuerSigningKeys = openIdConfig.SigningKeys
                };

            SecurityToken validatedToken;
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var user = handler.ValidateToken(accessToken, validationParameters, out validatedToken);
            if (user == null)
            {
                return new CustomGrantValidationResult("Invalid token");
            }

            return null;
        }

        private JWTParts ReconstructJWT(string accessToken)
        {
            var parts = accessToken.Split('.');
            var header = parts[0];
            var claims = parts[1];
            var signature = parts[2];

            var headerObj = JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(header)));
            var claimsObj = JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(claims)));

            return new JWTParts(headerObj, claimsObj, signature);
        }

        class JWTParts
        {
            public JWTParts(JObject headerObj, JObject claimsObj, string signature)
            {
                HeaderObj = headerObj;
                ClaimsObj = claimsObj;
                Signature = signature;
            }

            public JObject HeaderObj { get; }
            public JObject ClaimsObj { get; }
            public string Signature { get; }

            internal IEnumerable<Claim> ToClaims()
            {
                var list = new List<Claim>();
                foreach (var claim in ClaimsObj)
                {
                    list.Add(new Claim(claim.Key, claim.Value.Value<string>()));
                }
                return list;
            }
        }
    }

    public static class Base64Url
    {
        public static string Encode(byte[] arg)
        {
            string s3 = Convert.ToBase64String(arg);
            s3 = s3.Split('=')[0];
            s3 = s3.Replace('+', '-');
            return s3.Replace('/', '_');
        }

        public static byte[] Decode(string arg)
        {
            string s3 = arg;
            s3 = s3.Replace('-', '+');
            s3 = s3.Replace('_', '/');
            switch (s3.Length % 4)
            {
                case 2:
                    s3 += "==";
                    break;
                case 3:
                    s3 += "=";
                    break;
                default:
                    throw new Exception("Illegal base64url string!");
                case 0:
                    break;
            }
            return Convert.FromBase64String(s3);
        }
    }
}
