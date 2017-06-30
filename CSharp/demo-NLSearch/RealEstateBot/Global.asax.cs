using System.IO;
using System.Web;
using System.Web.Http;
using Autofac;
using Microsoft.Azure.Search.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Newtonsoft.Json;
using RealEstateBot.Dialogs;
using Search.Azure.Services;
using Search.Models;
using Search.Services;

namespace RealEstateBot
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<RealEstateDialog>()
                .As<IDialog<object>>()
                .InstancePerDependency();

            builder.Update(Conversation.Container);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}