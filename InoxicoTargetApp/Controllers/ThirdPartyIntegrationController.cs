using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace InoxicoTargetApp.Controllers
{
    public class ThirdPartyIntegrationController : Controller
    {
        private static string InoxicoIdentityStsClientAddress = "https://localhost:44301/connect/token";
        private static readonly HttpClient _httpClient = new HttpClient();

        [HttpPost]
        public async Task<string> AuthenticateExternalUser(string clientId)
        {
            var idToken = this.Request.Headers["id"];

            var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost:44301/api/ReferenceCode/{clientId}");
            request.Headers.Add("id", idToken);
            request.Content = new StringContent(string.Empty);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var refCode = await response.Content.ReadAsStringAsync();
            refCode = refCode.Replace("\"", string.Empty);

            return $"{this.Request.Url.Scheme}://{this.Request.Url.Authority}/ThirdPartyIntegration/AuthenticateExternalUserWithRefCode?refCode={refCode}";
        }

        public async Task<ActionResult> AuthenticateExternalUserWithRefCode(string refCode)
        {
            var client = new TokenClient(
                _httpClient,
                new TokenClientOptions()
                {
                    Address = InoxicoIdentityStsClientAddress,
                    ClientId = "external_ref_code_client",
                    ClientSecret = "secret1"
                });

            var token = await client.RequestTokenAsync("refcode_grant", new Dictionary<string, string>
            {
                {"refCode", refCode}
            });

            // Ignore contents of this method
            //var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost:44301/api/GetTokenUsingCode/{refCode}");
            //request.Content = new StringContent(string.Empty);

            //var response = await _httpClient.SendAsync(request);
            //response.EnsureSuccessStatusCode();

            //var token = await response.Content.ReadAsStringAsync();

            return Redirect("/");
        }
    }
}