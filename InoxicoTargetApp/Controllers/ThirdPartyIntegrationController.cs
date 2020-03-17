using IdentityModel.Client;
using System;
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

            return $"{this.Request.Url.Scheme}://{this.Request.Url.Authority}/{refCode}";
        }

        public async Task<ActionResult> AuthenticateExternalUserWithRefCode(string refCode)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"https://localhost:44301/api/GetTokenUsingCode/{refCode}");
            request.Content = new StringContent(string.Empty);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var token = await response.Content.ReadAsStringAsync();

            var client = new HttpClient();
            client.SetBearerToken(token);

            return Redirect("/");
        }
    }
}