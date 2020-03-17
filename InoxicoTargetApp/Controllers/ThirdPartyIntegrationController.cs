using IdentityModel.Client;
using System;
using System.Net.Http;
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
            var response = await _httpClient.PutAsync($"https://localhost:44301/api/ReferenceCode/{clientId}", new StringContent(string.Empty));
            response.EnsureSuccessStatusCode();
            var refCode = await response.Content.ReadAsStringAsync();
            return $"{this.Request.Url.Scheme}://{this.Request.Url.Authority}/{refCode}";
        }
    }
}