using System.Web.Mvc;

namespace WebSecurity.Controllers
{
    public class CustomErrorController : Controller
    {
        public ActionResult NotFound()
        {
            return View();
        }

        public ActionResult GeneralError()
        {
            return View();
        }
    }
}