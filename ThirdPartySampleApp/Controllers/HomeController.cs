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
        private const string InoxicoCoreBaseUrl = "https://localhost:44302/ThirdPartyIntegration/AuthenticateExternalUser";

        private static readonly HttpClient _httpClient = new HttpClient();

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<ActionResult> GoToInoxicoCore()
        {
            // Make call to get redirect url.
            var authProperties = HttpContext.GetOwinContext().Authentication.AuthenticateAsync("Cookies").Result;
            var idToken = authProperties.Properties.Dictionary.First(x => x.Key == "id_token");

            var redirectUrl = await AuthenticateUserToInoxicoCore(idToken.Value);

            //return Redirect(redirectUrl.Result);
            return Redirect("/");
        }

        private async Task<string> AuthenticateUserToInoxicoCore(string idTokenValue)
        {
            var response = await _httpClient.PostAsync(InoxicoCoreBaseUrl, new StringContent(idTokenValue));
            response.EnsureSuccessStatusCode();
            var refCode = await response.Content.ReadAsStringAsync();
            return $"{Request.UserHostAddress}/{refCode}";
        }

        private static string HttpGet(string URI, string idTokenValue)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
            req.Headers.Add(idTokenValue);
            //req.Proxy = new System.Net.WebProxy(ProxyString, true); //true means no proxy
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "You are logged in now. Here's your token:";

            /*var user = User as ClaimsPrincipal;
            var token = user.FindFirst("access_token");*/
            /*if (token != null)
            {
                ViewData["access_token"] = token.Value;
            }*/

            var authProperties = HttpContext.GetOwinContext().Authentication.AuthenticateAsync("Cookies").Result;

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