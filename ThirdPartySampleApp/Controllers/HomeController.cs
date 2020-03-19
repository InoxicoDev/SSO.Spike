using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Owin.Security;

namespace ThirdPartySampleApp.Controllers
{
    public class HomeController : Controller
    {
        private const string TargetAppBaseUrl = "https://localhost:44302";

        private static readonly HttpClient _httpClient = new HttpClient();

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> GoToTargetApp()
        {
            // Make call to get redirect url.
            var authProperties = HttpContext.GetOwinContext().Authentication.AuthenticateAsync("Cookies").Result;
            var idToken = authProperties.Properties.Dictionary.First(x => x.Key == "id_token");

            var redirectUrl = await AuthenticateUserToTargetApp(idToken.Value);

            return Redirect(redirectUrl);
        }

        private async Task<string> AuthenticateUserToTargetApp(string idTokenValue)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{TargetAppBaseUrl}/ThirdPartyIntegration/AuthenticateExternalUser?clientId=1234");
            request.Content = new StringContent(string.Empty);
            request.Headers.Add("id", idTokenValue);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var redirectUrl = await response.Content.ReadAsStringAsync();
            return redirectUrl;
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "You are logged in now. Here's your token:";

            var user = User as ClaimsPrincipal;
            var token = user.FindFirst("access_token");
            if (token != null)
            {
                ViewData["access_token"] = token.Value;
            }

            return View();
        }

        public ActionResult Signout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }

        public void SignoutCleanup(string sid)
        {
            var cp = (ClaimsPrincipal)User;
            var sidClaim = cp.FindFirst("sid");
            if (sidClaim != null && sidClaim.Value == sid)
            {
                Request.GetOwinContext().Authentication.SignOut("Cookies");
            }
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}