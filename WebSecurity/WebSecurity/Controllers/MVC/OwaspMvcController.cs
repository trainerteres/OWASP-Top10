using System.Web.Mvc;

namespace WebSecurity.Controllers
{
    public class OwaspMvcController : Controller
    {
        // GET: OwaspMvc
        public ActionResult Index()
        {
            return View();
        }
    }
}