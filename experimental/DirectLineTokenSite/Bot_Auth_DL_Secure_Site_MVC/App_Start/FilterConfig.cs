using System.Web;
using System.Web.Mvc;

namespace Bot_Auth_DL_Secure_Site_MVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
