using System.Web.Mvc;

namespace WebSecurity.Controllers
{
    public class OwaspMvc2013Controller : Controller
    {
        // GET: OwaspMvc
        public ActionResult Index()
        {
            return View();
        }
    }
}