using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace simpleSendMessage
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        

        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

         
        }
    }
}
