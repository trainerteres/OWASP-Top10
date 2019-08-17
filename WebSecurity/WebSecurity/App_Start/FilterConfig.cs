using System.Web;
using System.Web.Mvc;
using WebSecurity.Filter;

namespace WebSecurity
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new CustomAuthAttribute());
        }
    }
}
