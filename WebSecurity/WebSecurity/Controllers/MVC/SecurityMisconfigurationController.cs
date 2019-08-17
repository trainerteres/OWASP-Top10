using System.Web.Mvc;

namespace WebSecurity.Controllers
{
    public class SecurityMisconfigurationController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}