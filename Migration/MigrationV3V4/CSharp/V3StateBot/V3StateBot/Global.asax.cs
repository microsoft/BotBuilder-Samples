using System.Web.Http;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Autofac;
using Microsoft.Bot.Connector;
using System.Reflection;
using System;
using System.Configuration;

namespace V3StateBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            Conversation.UpdateContainer(
            builder =>
            {
                builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));

                // Bot Storage: Here we register the state storage for your bot. 
                // Default store: volatile in-memory store - Only for prototyping!
                // We provide adapters for Azure Table, CosmosDb, SQL Azure, or you can implement your own!
                // For samples and documentation, see: [https://github.com/Microsoft/BotBuilder-Azure](https://github.com/Microsoft/BotBuilder-Azure)
                //var store = new InMemoryDataStore();

                // Other storage options
                
                var store = new DocumentDbBotDataStore(new Uri(ConfigurationManager.AppSettings["CosmosEndpoint"]),
                                                        ConfigurationManager.AppSettings["CosmosKey"],
                                                        ConfigurationManager.AppSettings["CosmosDatataseName"],
                                                        ConfigurationManager.AppSettings["CosmosCollectionName"]);

                //var sqlConnection = ConfigurationManager.ConnectionStrings["BotDataContextConnectionString"].ConnectionString;
                //var store = new SqlBotDataStore(sqlConnection);
                

                //var tableStoreConnectionString = ConfigurationManager.ConnectionStrings["TableStorageConnectionString"].ConnectionString;
                //var store = new TableBotDataStore(tableStoreConnectionString);
                //var store = new TableBotDataStore2(tableStoreConnectionString);


                builder.Register(c => store)
                    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                    .AsSelf()
                    .SingleInstance();
            });
        }
    }
}
