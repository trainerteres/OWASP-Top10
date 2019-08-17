using System.Security.Claims;
using System.Web.Mvc;
using WebSecurity.Common;
using WebSecurity.ReverseEngineering;

namespace WebSecurity.Controllers
{
    public class BrokenAuthenticationController : Controller
    {
        [Authorize]
        // GET: BrokenAuthentication
        public ActionResult Index()
        {
            //var cookieContent = Request.Cookies[".AspNet.ApplicationCookie"];

            //var authType = string.Empty;
            //var claims = CookieExtractor.ExtractClaimsFromCookie(cookieContent.Value, ref authType);

            //var cookie = CookieExtractor.CreateCookie(claims,authType);

            ViewData["UserName"] = "TestUser";

            return View();
        }


    }
}