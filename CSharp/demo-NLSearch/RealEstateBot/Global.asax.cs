using System.Web.Http;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using RealEstateBot.Dialogs;
using Search.Dialogs;
using System.Configuration;
using System;
using System.Web;

namespace RealEstateBot
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();

            var translationKey = ConfigurationManager.AppSettings["TranslationKey"];
            if (string.IsNullOrWhiteSpace(translationKey))
            {
                translationKey = Environment.GetEnvironmentVariable("TranslationKey");
            }
            builder.RegisterInstance<SearchTranslator>(new SearchTranslator("en", translationKey))
                   .AsImplementedInterfaces()
                    .SingleInstance();

            /* builder.RegisterType<TraceActivityLogger>()
                .AsImplementedInterfaces()
                .SingleInstance();*/

            builder.RegisterType<RealEstateDialog>()
                .As<IDialog<object>>()
                .InstancePerDependency();

            builder.Update(Conversation.Container);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}