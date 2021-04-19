// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.3.0

using Bot.Builder.Azure.V3V4;
using Bot.Builder.Azure.V3V4.AzureSql;
using Bot.Builder.Azure.V3V4.AzureTable;
using Bot.Builder.Azure.V3V4.CosmosDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System;
using V4StateBot.Bots;
using V4StateBot.Dialogs;

namespace V4StateBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Create Conversation State using MemoryStorage (conversations will be reset on server restart)
            IStorage conversationStorage = new MemoryStorage();
            var conversationState = new ConversationState(conversationStorage);
            services.AddSingleton<ConversationState>(conversationState);


            //Uri docDbEmulatorUri = new Uri("https://localhost:8081");
            //const string docDbEmulatorKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

            // DocumentDbBotDataStore for V3V4 User State
            Uri docDbEmulatorUri = new Uri(Configuration["v3CosmosEndpoint"]);
            var documentDbBotDataStore = new DocumentDbBotDataStore(docDbEmulatorUri,
                        Configuration["v3CosmosKey"],
                        databaseId: Configuration["v3CosmosDatataseName"],
                        collectionId: Configuration["v3CosmosCollectionName"]);

            // SqlBotDataStore for V3V4 User State
            //var sqlConnectionString = Configuration.GetConnectionString("SqlBotData");
            //var sqlBotDataStore = new SqlBotDataStore(sqlConnectionString);

            // TableBotDataStore for V3V4 User State
            //var tableConnectionString = Configuration.GetConnectionString("AzureTable");
            //var tableBotDataStore = new TableBotDataStore(tableConnectionString);

            // TableBotDataStore2 for V3V4 User State            
            //var tableBotDataStore2 = new TableBotDataStore2(tableConnectionString);


            // Create the V3V4Storage layer bridge, providing a V3 storage.
            // Then use that storage to create a V3V4State, and inject as a singleton
            var v3v4Storage = new V3V4Storage(documentDbBotDataStore);
            //var v3v4Storage = new V3V4Storage(sqlBotDataStore);
            //var v3v4Storage = new V3V4Storage(tableBotDataStore);
            //var v3v4Storage = new V3V4Storage(tableBotDataStore2);
            services.AddSingleton<V3V4State>(new V3V4State(v3v4Storage));
            
            // Create the credential provider to be used with the Bot Framework Adapter.
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            services.AddSingleton<MainDialog>();
            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, EchoBot<MainDialog>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
