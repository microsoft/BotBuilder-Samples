namespace CustomStateBot
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Web.Http;
    using Autofac;
    using Microsoft.Bot.Builder.Azure;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Uri docDbServiceEndpoint = new Uri(ConfigurationManager.AppSettings["DocumentDbServiceEndpoint"]);
            string docDbEmulatorKey = ConfigurationManager.AppSettings["DocumentDbAuthKey"];

            Conversation.UpdateContainer(builder =>
            {
                builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

                var store = new DocumentDbBotDataStore(docDbServiceEndpoint, docDbEmulatorKey);
                builder.Register(c => store)
                    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                    .AsSelf()
                    .SingleInstance();

            });

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
