using IdentityModel.Client;
using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace InoxicoCoreSample.Controllers
{
    public class HomeController : Controller
    {
        private const string AuthorizeEndpoint = "https://localhost:44333/core/connect/authorize";

        public ActionResult Index()
        {
            //Request.GetOwinContext().Authentication.SignIn();

            return View();
        }

        [HttpPost]
        public ActionResult Index(string scopes)
        {
            var state = Guid.NewGuid().ToString("N");
            var nonce = Guid.NewGuid().ToString("N");
            SetTempState(state, nonce);

            var request = new IdentityModel.Client.RequestUrl(AuthorizeEndpoint);

            var url = request.CreateAuthorizeUrl(
                clientId: "codeclient",
                responseType: "code",
                scope: "openid profile read write offline_access",
                redirectUri: "https://localhost:44312/callback",
                //prompt: "none",
                state: state,
                nonce: nonce);

            return Redirect("https://google.com");

            //return Redirect(url);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private void SetTempState(string state, string nonce)
        {
            var tempId = new ClaimsIdentity("TempState");
            tempId.AddClaim(new Claim("state", state));
            tempId.AddClaim(new Claim("nonce", nonce));

            Request.GetOwinContext().Authentication.SignIn(tempId);
        }
    }
}