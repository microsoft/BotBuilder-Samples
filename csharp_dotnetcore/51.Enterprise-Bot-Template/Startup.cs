// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EnterpriseBot
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
            var botConfig = BotConfiguration.LoadAsync(@".\EnterpriseBot.bot").GetAwaiter().GetResult();
            services.AddSingleton(sp => botConfig);

            // Initializes your bot service clients and adds a singleton that your Bot can access through dependency injection.
            var connectedServices = InitBotServices(botConfig);
            services.AddSingleton(sp => connectedServices);

            // Initializes Bot Conversation and User State and adds a singleton that your Bot can access through dependency injection.
            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
                var userState = options.State.OfType<UserState>().FirstOrDefault();

                var accessors = new EnterpriseBotAccessors
                {
                    ConversationDialogState = conversationState.CreateProperty<DialogState>("DialogState"),
                };

                return accessors;
            });

            services.AddBot<EnterpriseBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(this.Configuration);

                // Telemetry Middleware (logs activity messages in Application Insights)
                var appInsightsService = botConfig.Services.Where(s => s.Type == ServiceTypes.AppInsights).FirstOrDefault();
                if (appInsightsService != null)
                {
                    var instrumentationKey = (appInsightsService as AppInsightsService).InstrumentationKey;
                    var appInsightsLogger = new TelemetryLoggerMiddleware(instrumentationKey, logUserName: true, logOriginalMessage: true);
                    options.Middleware.Add(appInsightsLogger);
                }
                else
                {
                    throw new Exception("Please configure your AppInsights connection in your .bot file.");
                }

                // Catches any errors that occur during a conversation turn and logs them to AppInsights.
                options.OnTurnError = async (context, exception) =>
                {
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                    connectedServices.TelemetryClient.TrackException(exception);
                };

                // Bot State Middleware
                var cosmosDbService = botConfig.Services.Where(s => s.Type == ServiceTypes.CosmosDB).FirstOrDefault();
                if (cosmosDbService != null)
                {
                    var cosmosDb = cosmosDbService as CosmosDbService;
                    var connectionStringItems = cosmosDb.ConnectionString.Split(";");
                    var cosmosOptions = new CosmosDbStorageOptions()
                    {
                        CosmosDBEndpoint = new Uri(connectionStringItems[0].Split("=")[1]),
                        AuthKey = connectionStringItems[1].Split("=")[1] + "==",
                        CollectionId = cosmosDb.Collection,
                        DatabaseId = cosmosDb.Database,
                    };

                    IStorage datastore = new CosmosDbStorage(cosmosOptions);
                    options.State.Add(new ConversationState(datastore));
                    options.Middleware.Add(new BotStateSet(options.State.ToArray()));
                }
                else
                {
                    throw new Exception("Please configure your CosmosDb connection in your .bot file.");
                }

                // Transcript Middleware (saves conversation history in a standard format)
                var storageService = botConfig.Services.Where(s => s.Type == ServiceTypes.BlobStorage).FirstOrDefault();
                if (storageService != null)
                {
                    var blobStorage = storageService as BlobStorageService;
                    var transcriptStore = new AzureBlobTranscriptStore(blobStorage.ConnectionString, blobStorage.Container);
                    var transcriptMiddleware = new TranscriptLoggerMiddleware(transcriptStore);
                    options.Middleware.Add(transcriptMiddleware);
                }

                // Typing Middleware (automatically shows typing when the bot is responding/working)
                var typingMiddleware = new ShowTypingMiddleware();
                options.Middleware.Add(typingMiddleware);

                // Content Moderation Middleware (analyzes incoming messages for inappropriate content including PII, profanity, etc.)
                var moderatorService = botConfig.Services.Where(s => s.Name == ContentModeratorMiddleware.ServiceName).FirstOrDefault();
                if (moderatorService != null)
                {
                    var moderator = moderatorService as GenericService;
                    var moderatorKey = moderator.Configuration["subscriptionKey"];
                    var moderatorRegion = moderator.Configuration["region"];
                    var moderatorMiddleware = new ContentModeratorMiddleware(moderatorKey, moderatorRegion);
                    options.Middleware.Add(moderatorMiddleware);
                }
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application Builder.</param>
        /// <param name="env">Hosting Environment.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }

        /// <summary>
        /// Initializes service clients which will be used throughout the bot code into a single object.
        /// It is recommended that you add any additional service clients you may need into the <see cref="BotServices"/> object and initialize them here.
        /// These services include AppInsights telemetry client, Luis Recognizers, QnAMaker instances, etc.</summary>
        /// <param name="config">Bot configuration object based on .bot json file.</param>
        /// <returns>BotServices object.</returns>
        private BotServices InitBotServices(BotConfiguration config)
        {
            var connectedServices = new BotServices();

            foreach (var service in config.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.AppInsights:
                        {
                            if (connectedServices.DispatchRecognizer != null)
                            {
                                throw new Exception("Only one telemetry client can currently be configured in BotServices.");
                            }

                            var appInsights = service as AppInsightsService;
                            var telemetryConfig = new TelemetryConfiguration(appInsights.InstrumentationKey);

                            connectedServices.TelemetryClient = new TelemetryClient(telemetryConfig);
                            break;
                        }

                    case ServiceTypes.Dispatch:
                        {
                            if (connectedServices.DispatchRecognizer != null)
                            {
                                throw new Exception("Only one dispatch service can currently be configured in BotServices.");
                            }

                            var dispatch = service as DispatchService;
                            var dispatchApp = new LuisApplication(dispatch.AppId, dispatch.SubscriptionKey, dispatch.Region);
                            var dispatchRecognizer = new TelemetryLuisRecognizer(dispatchApp);

                            connectedServices.DispatchRecognizer = dispatchRecognizer;
                            break;
                        }

                    case ServiceTypes.Luis:
                        {
                            var luis = service as LuisService;
                            var luisApp = new LuisApplication(luis.AppId, luis.SubscriptionKey, luis.Region);
                            var luisRecognizer = new TelemetryLuisRecognizer(luisApp);

                            connectedServices.LuisServices.Add(luis.Name, luisRecognizer);
                            break;
                        }

                    case ServiceTypes.QnA:
                        {
                            var qna = service as QnAMakerService;
                            var qnaEndpoint = new QnAMakerEndpoint()
                            {
                                KnowledgeBaseId = qna.KbId,
                                EndpointKey = qna.EndpointKey,
                                Host = qna.Hostname,
                            };
                            var qnaMaker = new TelemetryQnAMaker(qnaEndpoint);
                            connectedServices.QnAServices.Add(qna.Name, qnaMaker);

                            break;
                        }

                    case ServiceTypes.Generic:
                        {
                            // update readme with .bot update instructions
                            if (service.Name == "Authentication")
                            {
                                var authentication = service as GenericService;
                                connectedServices.AuthConnectionName = authentication.Configuration["Azure Active Directory v2"];
                            }

                            break;
                        }
                }
            }

            return connectedServices;
        }
    }
}
