using System;
using System.Linq;
using System.Security.Claims;
using System.Web.Mvc;
using WebSecurity.Common;
using WebSecurity.Encryption;

namespace WebSecurity.Controllers
{
    public class InsecureObjRefController : Controller
    {
        // GET: InsecureObjRef
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult InsecuredDirectObjRef(int AccountNumber)
        {
            var accountNumber = InMemoryRepository.AccountNumber.FirstOrDefault(x => x.Number == AccountNumber);
            return View("AccountDetails", accountNumber);
        }

        [Authorize]
        public ActionResult GetDetailsWithUserRestriction(int AccountNumber)
        {
            var userName = User.Identity.Name;
            var roles = ((ClaimsIdentity)User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value).ToArray();


            var accountNumber = InMemoryRepository.RestrictedAccountNumbers(roles, userName).FirstOrDefault(x => x.Number == AccountNumber);

            return View("AccountDetails", accountNumber);
        }


        [Authorize]
        public ActionResult StaticIndirectReferenceMap(string AccountNumber)
        {
            var accountId = Convert.ToInt32(StaticReferenceMap.GetDirectReferenceMap(AccountNumber));
            var userName = User.Identity.Name;
            var roles = ((ClaimsIdentity)User.Identity).Claims
                 .Where(c => c.Type == ClaimTypes.Role)
                 .Select(c => c.Value).ToArray();

            var accountNumber = InMemoryRepository.RestrictedAccountNumbers(roles, userName).FirstOrDefault(x => x.Number == accountId);
            return View("AccountDetails", accountNumber);
        }

    }
}