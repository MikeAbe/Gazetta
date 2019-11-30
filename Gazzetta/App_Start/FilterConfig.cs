using System.Web;
using System.Web.Mvc;

namespace Gazzetta
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //Restrict everything to logged in user
            //filters.Add(new AuthorizeAttribute());
        }
    }
}
