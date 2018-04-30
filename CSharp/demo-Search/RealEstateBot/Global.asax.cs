namespace RealEstateBot
{
    using System.Reflection;
    using System.Web;
    using System.Web.Http;
    using Autofac;
    using Microsoft.Azure.Search.Models;
    using Microsoft.Bot.Builder.Azure;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Connector;

    using RealEstateBot.Dialogs;
    using Search.Azure.Services;
    using Search.Models;
    using Search.Services;

    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            Conversation.UpdateContainer(builder =>
            {
                builder.RegisterType<IntroDialog>()
                  .As<IDialog<object>>()
                  .InstancePerDependency();

                builder.RegisterType<RealEstateMapper>()
                   .Keyed<IMapper<DocumentSearchResult, GenericSearchResult>>(FiberModule.Key_DoNotSerialize)
                   .AsImplementedInterfaces()
                   .SingleInstance();

                builder.RegisterType<AzureSearchClient>()
                    .Keyed<ISearchClient>(FiberModule.Key_DoNotSerialize)
                    .AsImplementedInterfaces()
                    .SingleInstance();

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

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}