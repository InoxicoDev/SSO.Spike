using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Claims;

namespace ThirdPartySampleApp.Controllers
{
    public class HomeController : Controller
    {
        private const string SampleCoreBaseAddress = "http://localhost:57255/";

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string coreUrl = null)
        {
            return Redirect("Home/About");
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

        [Authorize, HttpPost]
        public ActionResult About(string stuff = null)
        {
            return Redirect(SampleCoreBaseAddress);
        }

        public ActionResult Signout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}