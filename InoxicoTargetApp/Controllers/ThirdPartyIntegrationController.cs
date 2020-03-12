using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InoxicoTargetApp.Controllers
{
    public class ThirdPartyIntegrationController : Controller
    {
        public ActionResult GetRedirectUrl(string clientId)
        {

            return Json(new { Url = "" });
        }
    }
}