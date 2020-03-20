using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Text;
using Common;

namespace InoxicoTargetApp.Controllers
{
    public class ThirdPartyIntegrationController : Controller
    {
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

                var requestUrl = new RequestUrl(Addresses.InoxicoSTSAuth);
                var redirectUrl = requestUrl.CreateAuthorizeUrl(
                    clientId: OAuth.InoxicoClientId,
                    responseType: "id_token token",
                    scope: "openid profile read write offline_access",
                    redirectUri: Addresses.IntendedLocation,
                    state: EncodeState(idToken, clientId),
                    nonce: Guid.NewGuid().ToString("N"),
                    acrValues: $"idp:{OAuth.IdentityProvider_ThirdParty}");

                return redirectUrl;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        private static string EncodeState(string token, string clientId)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId};{token}"));
        }
    }
}