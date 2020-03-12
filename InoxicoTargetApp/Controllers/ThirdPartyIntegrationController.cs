using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace InoxicoTargetApp.Controllers
{
    public class ThirdPartyIntegrationController : Controller
    {
        public ActionResult GetRedirectUrl()
        {
            var state = Guid.NewGuid().ToString("N");
            var nonce = Guid.NewGuid().ToString("N");

            var tempId = new ClaimsIdentity("TempState");
            tempId.AddClaim(new Claim("state", state));
            tempId.AddClaim(new Claim("nonce", nonce));

            Request.GetOwinContext().Authentication.SignIn(tempId);

            var requestUrl = new RequestUrl("https://localhost:44301/connect/authorize");

            var url = requestUrl.CreateAuthorizeUrl(
                clientId: "codeclient",
                responseType: "code",
                scope: "openid profile read write offline_access",
                redirectUri: "https://localhost:44302/callback",
                state: state,
                nonce: nonce);

            return Json(new { Url = url }, JsonRequestBehavior.AllowGet);
        }
    }
}