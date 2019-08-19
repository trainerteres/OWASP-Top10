using System.Web.Mvc;

namespace WebSecurity.Controllers
{
    public class OwaspMvc2017Controller : Controller
    {
        // GET: OwaspMvc
        public ActionResult Index()
        {
            return View();
        }
    }
}