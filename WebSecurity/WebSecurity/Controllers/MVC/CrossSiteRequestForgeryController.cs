using System.Web.Mvc;

namespace WebSecurity.Controllers
{
    public class CrossSiteRequestForgeryController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}