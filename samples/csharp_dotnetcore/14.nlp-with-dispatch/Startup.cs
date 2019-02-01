// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace NLP_With_Dispatch_Bot
{
    /// <summary>
    /// The Startup class configures services and the app's request pipeline.
    /// </summary>
    public class Startup
    {
        private ILoggerFactory _loggerFactory;
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

        /// <summary>
        /// Gets the configuration that represents a set of key/value application configuration properties.
        /// </summary>
        /// <value>
        /// The <see cref="IConfiguration"/> that represents a set of key/value application configuration properties.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        /// <seealso cref="IServiceCollection"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/dependency-injection"/>
        public void ConfigureServices(IServiceCollection services)
        {
            var secretKey = Configuration.GetSection("botFileSecret")?.Value;
            var botFilePath = Configuration.GetSection("botFilePath")?.Value;
            if (!File.Exists(botFilePath))
            {
                throw new FileNotFoundException($"The .bot configuration file was not found. botFilePath: {botFilePath}");
            }

            // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
            var botConfig = BotConfiguration.Load(botFilePath ?? @".\nlp-with-dispatch.bot", secretKey);
            services.AddSingleton(sp => botConfig ?? throw new InvalidOperationException($"The .bot configuration file could not be loaded. botFilePath: {botFilePath}"));

            // Retrieve current endpoint.
            var environment = _isProduction ? "production" : "development";
            var service = botConfig.Services.FirstOrDefault(s => s.Type == "endpoint" && s.Name == environment);
            if (!(service is EndpointService endpointService))
            {
                throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{environment}'.");
            }

            var connectedServices = InitBotServices(botConfig);

            services.AddSingleton(sp => connectedServices);

            services.AddBot<NlpDispatchBot>(options =>
            {

                options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

                // Creates a logger for the application to use.
                ILogger logger = _loggerFactory.CreateLogger<NlpDispatchBot>();

                // Catches any errors that occur during a conversation turn and logs them.
                options.OnTurnError = async (context, exception) =>
                {
                    logger.LogError($"Exception caught : {exception}");
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };

                // The Memory Storage used here is for local bot debugging only. When the bot
                // is restarted, everything stored in memory will be gone.
                IStorage dataStore = new MemoryStorage();

                // For production bots use the Azure Blob or
                // Azure CosmosDB storage providers. For the Azure
                // based storage providers, add the Microsoft.Bot.Builder.Azure
                // Nuget package to your solution. That package is found at:
                // https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/
                // Uncomment the following lines to use Azure Blob Storage
                // //Storage configuration name or ID from the .bot file.
                // const string StorageConfigurationId = "<STORAGE-NAME-OR-ID-FROM-BOT-FILE>";
                // var blobConfig = botConfig.FindServiceByNameOrId(StorageConfigurationId);
                // if (!(blobConfig is BlobStorageService blobStorageConfig))
                // {
                //    throw new InvalidOperationException($"The .bot file does not contain an blob storage with name '{StorageConfigurationId}'.");
                // }
                // // Default container name.
                // const string DefaultBotContainer = "<DEFAULT-CONTAINER>";
                // var storageContainer = string.IsNullOrWhiteSpace(blobStorageConfig.Container) ? DefaultBotContainer : blobStorageConfig.Container;
                // IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureBlobStorage(blobStorageConfig.ConnectionString, storageContainer);

                // Create Conversation State object.
                // The Conversation State object is where we persist anything at the conversation-scope.
                var conversationState = new ConversationState(dataStore);

                options.State.Add(conversationState);
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }

        /// <summary>
        /// Initialize the bot's references to external services.
        ///
        /// For example, QnaMaker services are created here.
        /// These external services are configured
        /// using the <see cref="BotConfiguration"/> class (based on the contents of your ".bot" file).
        /// </summary>
        /// <param name="config"><see cref="BotConfiguration"/> object based on your ".bot" file.</param>
        /// <returns>A <see cref="BotServices"/> representing client objects to access external services the bot uses.</returns>
        /// <seealso cref="BotConfiguration"/>
        /// <seealso cref="QnAMaker"/>
        /// <seealso cref="TelemetryClient"/>
        private static BotServices InitBotServices(BotConfiguration config)
        {
            var qnaServices = new Dictionary<string, QnAMaker>();
            var luisServices = new Dictionary<string, LuisRecognizer>();

            foreach (var service in config.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.Luis:
                        {
                            // Create a Luis Recognizer that is initialized and suitable for passing
                            // into the IBot-derived class (NlpDispatchBot).
                            // In this case, we're creating a custom class (wrapping the original
                            // Luis Recognizer client) that logs the results of Luis Recognizer results
                            // into Application Insights for future analysis.
                            if (!(service is LuisService luis))
                            {
                                throw new InvalidOperationException("The LUIS service is not configured correctly in your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(luis.AppId))
                            {
                                throw new InvalidOperationException("The LUIS Model Application Id ('appId') is required to run this sample. Please update your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(luis.AuthoringKey))
                            {
                                throw new InvalidOperationException("The Luis Authoring Key ('authoringKey') is required to run this sample. Please update your '.bot' file.");
                            }

                            // CAUTION: Authoring key is used in this example as it is appropriate for prototyping.
                            // When implimenting for deployment/production, assign and use a subscription key instead of an authoring key.
                            // if (string.IsNullOrWhiteSpace(luis.SubscriptionKey))
                            // {
                            //     throw new InvalidOperationException("The Subscription Key ('subscriptionKey') is required to run this sample. Please update your '.bot' file.");
                            // }

                            if (string.IsNullOrWhiteSpace(luis.Region))
                            {
                                throw new InvalidOperationException("The Region ('region') is required to run this sample. Please update your '.bot' file.");
                            }

                            var app = new LuisApplication(luis.AppId, luis.AuthoringKey, luis.GetEndpoint());
                            var recognizer = new LuisRecognizer(app);
                            luisServices.Add(luis.Name, recognizer);
                            break;
                        }

                    case ServiceTypes.Dispatch:
                        // Create a Dispatch Recognizer that is initialized and suitable for passing
                        // into the IBot-derived class (NlpDispatchBot).
                        // In this case, we're creating a custom class (wrapping the original
                        // Luis Recognizer client) that logs the results of Luis Recognizer results
                        // into Application Insights for future analysis.
                        if (!(service is DispatchService dispatch))
                        {
                            throw new InvalidOperationException("The Dispatch service is not configured correctly in your '.bot' file.");
                        }

                        if (string.IsNullOrWhiteSpace(dispatch.AppId))
                        {
                            throw new InvalidOperationException("The LUIS Model Application Id ('appId') is required to run this sample. Please update your '.bot' file.");
                        }

                        if (string.IsNullOrWhiteSpace(dispatch.AuthoringKey))
                        {
                            throw new InvalidOperationException("The LUIS Authoring Key ('authoringKey') is required to run this sample. Please update your '.bot' file.");
                        }

                        if (string.IsNullOrWhiteSpace(dispatch.SubscriptionKey))
                        {
                            throw new InvalidOperationException("The Subscription Key ('subscriptionKey') is required to run this sample. Please update your '.bot' file.");
                        }

                        if (string.IsNullOrWhiteSpace(dispatch.Region))
                        {
                            throw new InvalidOperationException("The Region ('region') is required to run this sample. Please update your '.bot' file.");
                        }

                        // CAUTION: Authoring key is used in this example as it is appropriate for prototyping.
                        // When implimenting for deployment/production, assign and use a subscription key instead of an authoring key.
                        var dispatchApp = new LuisApplication(dispatch.AppId, dispatch.AuthoringKey, dispatch.GetEndpoint());

                        // Since the Dispatch tool generates a LUIS model, we use LuisRecognizer to resolve dispatching of the incoming utterance
                        var dispatchARecognizer = new LuisRecognizer(dispatchApp);
                        luisServices.Add(dispatch.Name, dispatchARecognizer);
                        break;

                    case ServiceTypes.QnA:
                        {
                            // Create a QnA Maker that is initialized and suitable for passing
                            // into the IBot-derived class (NlpDispatchBot).
                            // In this case, we're creating a custom class (wrapping the original
                            // QnAMaker client) that logs the results of QnA Maker into Application
                            // Insights for future analysis.
                            if (!(service is QnAMakerService qna))
                            {
                                throw new InvalidOperationException("The QnA service is not configured correctly in your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(qna.KbId))
                            {
                                throw new InvalidOperationException("The QnA KnowledgeBaseId ('kbId') is required to run this sample. Please update your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(qna.EndpointKey))
                            {
                                throw new InvalidOperationException("The QnA EndpointKey ('endpointKey') is required to run this sample. Please update your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(qna.Hostname))
                            {
                                throw new InvalidOperationException("The QnA Host ('hostname') is required to run this sample. Please update your '.bot' file.");
                            }

                            var qnaEndpoint = new QnAMakerEndpoint()
                            {
                                KnowledgeBaseId = qna.KbId,
                                EndpointKey = qna.EndpointKey,
                                Host = qna.Hostname,
                            };

                            var qnaMaker = new QnAMaker(qnaEndpoint);
                            qnaServices.Add(qna.Name, qnaMaker);

                            break;
                        }
                }
            }

            return new BotServices(qnaServices, luisServices);
        }
    }
}
