using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InoxicoTargetApp.Controllers
{
    public class IntendedLocationController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}