using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Text;

namespace InoxicoTargetApp.Controllers
{
    public class ThirdPartyIntegrationController : Controller
    {
        private const string InoxicoStsAuthAddress = "https://localhost:44301/connect/authorize";
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

                var requestUrl = new RequestUrl(InoxicoStsAuthAddress);
                var redirectUrl = requestUrl.CreateAuthorizeUrl(
                    clientId: "inox_login",
                    responseType: "id_token token",
                    scope: "openid profile read write offline_access",
                    redirectUri: "https://localhost:44302/IntendedLocation",
                    state: EncodeState(idToken, clientId),
                    nonce: Guid.NewGuid().ToString("N"),
                    acrValues: "idp:ThirdParty");

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