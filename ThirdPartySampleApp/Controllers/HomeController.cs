using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;
using IdentityModel.Client;

namespace ThirdPartySampleApp.Controllers
{
    public class HomeController : Controller
    {
        private const string InoxicoIdentityAuthorizeRequest = "https://localhost:44333/core/connect/authorize";

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult GoToInoxicoCore()
        {
            var redirectUrl = "/";
            // Make call to get redirect url.
            var state = Guid.NewGuid().ToString("N");
            var nonce = Guid.NewGuid().ToString("N");
            //SetTempState(state, nonce);
            var user = User as ClaimsPrincipal;
            var token = user.Identity;

            var request = new RequestUrl(InoxicoIdentityAuthorizeRequest)
                .CreateAuthorizeUrl(
                    clientId: "codeclient",
                    responseType: "code",
                    scope: "openid email read profile",
                    redirectUri: "https://localhost:44304/",
                    state: state,
                    nonce: nonce,
                    prompt: "none"); //extra: token)

            var response = HttpGet(request);

            return Redirect(redirectUrl);
        }

        private static string HttpGet(string URI)
        {
            System.Net.WebRequest req = System.Net.WebRequest.Create(URI);
            //req.Proxy = new System.Net.WebProxy(ProxyString, true); //true means no proxy
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            return sr.ReadToEnd().Trim();
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