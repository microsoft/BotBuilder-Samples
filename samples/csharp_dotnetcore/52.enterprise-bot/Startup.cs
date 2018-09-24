// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnterpriseBot
{
    public class Startup
    {
        private bool _isProduction = false;

        public Startup(IHostingEnvironment env)
        {
            _isProduction = env.IsProduction();
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Load the connected services from .bot file.
            var botFilePath = Configuration.GetSection("botFilePath")?.Value;
            var botFileSecret = Configuration.GetSection("botFileSecret")?.Value;
            var botConfig = BotConfiguration.Load(botFilePath ?? @".\BotConfiguration.bot", botFileSecret);
            services.AddSingleton(sp => botConfig ?? throw new InvalidOperationException($"The .bot config file could not be loaded."));

            // Initializes your bot service clients and adds a singleton that your Bot can access through dependency injection.
            var connectedServices = new BotServices(botConfig);
            services.AddSingleton(sp => connectedServices);

            // Initialize Bot State
            var cosmosDbService = botConfig.Services.FirstOrDefault(s => s.Type == ServiceTypes.CosmosDB) ?? throw new Exception("Please configure your CosmosDb service in your .bot file.");
            var cosmosDb = cosmosDbService as CosmosDbService;
            var cosmosOptions = new CosmosDbStorageOptions()
            {
                CosmosDBEndpoint = new Uri(cosmosDb.Endpoint),
                AuthKey = cosmosDb.Key,
                CollectionId = cosmosDb.Collection,
                DatabaseId = cosmosDb.Database,
            };
            var dataStore = new CosmosDbStorage(cosmosOptions);
            var userState = new UserState(dataStore);
            var conversationState = new ConversationState(dataStore);

            services.AddSingleton(dataStore);
            services.AddSingleton(userState);
            services.AddSingleton(conversationState);
            services.AddSingleton(new BotStateSet(userState, conversationState));

            // Add the bot with options
            services.AddBot<EnterpriseBot>(options =>
            {
                // Load the connected services from .bot file.
                var environment = _isProduction ? "production" : "development";
                var service = botConfig.Services.FirstOrDefault(s => s.Type == ServiceTypes.Endpoint && s.Name == environment);
                if (!(service is EndpointService endpointService))
                {
                    throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{environment}'.");
                }

                options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

                // Telemetry Middleware (logs activity messages in Application Insights)
                var appInsightsService = botConfig.Services.FirstOrDefault(s => s.Type == ServiceTypes.AppInsights) ?? throw new Exception("Please configure your AppInsights connection in your .bot file.");
                var instrumentationKey = (appInsightsService as AppInsightsService).InstrumentationKey;
                var appInsightsLogger = new TelemetryLoggerMiddleware(instrumentationKey, logUserName: true, logOriginalMessage: true);
                options.Middleware.Add(appInsightsLogger);

                // Catches any errors that occur during a conversation turn and logs them to AppInsights.
                options.OnTurnError = async (context, exception) =>
                {
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                    connectedServices.TelemetryClient.TrackException(exception);
                };

                // Transcript Middleware (saves conversation history in a standard format)
                var storageService = botConfig.Services.FirstOrDefault(s => s.Type == ServiceTypes.BlobStorage) ?? throw new Exception("Please configure your Azure Storage service in your .bot file.");
                var blobStorage = storageService as BlobStorageService;
                var transcriptStore = new AzureBlobTranscriptStore(blobStorage.ConnectionString, blobStorage.Container);
                var transcriptMiddleware = new TranscriptLoggerMiddleware(transcriptStore);
                options.Middleware.Add(transcriptMiddleware);

                // Typing Middleware (automatically shows typing when the bot is responding/working)
                var typingMiddleware = new ShowTypingMiddleware();
                options.Middleware.Add(typingMiddleware);

                options.Middleware.Add(new AutoSaveStateMiddleware(userState, conversationState));
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application Builder.</param>
        /// <param name="env">Hosting Environment.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }
    }
}
