using System.Web.Http;

namespace startNewDialogWithPrompt
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);    
        }
    }
}
