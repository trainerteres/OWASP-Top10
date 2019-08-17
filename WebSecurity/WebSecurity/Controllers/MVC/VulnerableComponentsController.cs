using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSecurity.Controllers.MVC
{
    public class VulnerableComponentsController : Controller
    {
        // GET: VulnerableComponents
        // dependency-check.bat --project "Web Security" --scan "C:\Raxit\Temp\WebSecurity1\WebSecurity\WebSecurity\WebSecurity\bin"
        public ActionResult Index()
        {
            return View();
        }
    }
}