using System.Reflection;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Connector;

namespace TestBot
{
    using System.Web.Http;
    using System.Web.Routing;
    using Autofac;
    using Helpers;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.History;

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            this.RegisterBotDependencies();

            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        private void RegisterBotDependencies()
        {
            Conversation.UpdateContainer(builder =>
            {
                builder
                    .Register(c => new ActivityLogger(c.Resolve<IBotData>()))
                    .As<IActivityLogger>()
                    .InstancePerLifetimeScope();

                foreach (var commandType in CommandsHelper.GetRegistrableTypes())
                {
                    builder
                        .RegisterType(commandType)
                        .Keyed(commandType.Name, commandType)
                        .AsImplementedInterfaces()
                        .InstancePerMatchingLifetimeScope(DialogModule.LifetimeScopeTag);
                }

                builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

                // Bot Storage: Here we register the state storage for your bot. 
                // Default store: volatile in-memory store - Only for prototyping!
                // We provide adapters for Azure Table, CosmosDb, SQL Azure, or you can implement your own!
                // For samples and documentation, see: https://github.com/Microsoft/BotBuilder-Azure
                var store = new InMemoryDataStore();

                // Other storage options
                // var store = new TableBotDataStore("...DataStorageConnectionString..."); // requires Microsoft.BotBuilder.Azure Nuget package 
                // var store = new DocumentDbBotDataStore("cosmos db uri", "cosmos db key"); // requires Microsoft.BotBuilder.Azure Nuget package 

                builder.Register(c => store)
                    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                    .AsSelf()
                    .SingleInstance();
            });
        }
    }
}