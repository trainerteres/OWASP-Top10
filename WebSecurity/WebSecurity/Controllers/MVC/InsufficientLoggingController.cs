using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSecurity.Controllers.MVC
{
    public class InsufficientLoggingController : Controller
    {
        // GET: InsufficientLogging
        public ActionResult Index()
        {
            return View();
        }
    }
}