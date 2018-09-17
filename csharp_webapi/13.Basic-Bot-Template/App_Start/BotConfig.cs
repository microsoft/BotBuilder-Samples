// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.WebApi;
using Microsoft.Bot.Configuration;
using Unity;
using Unity.Lifetime;

namespace BasicBot
{
    /// <summary>
    /// Performs the Bot-specific configuration during Asp.Net start.
    /// </summary>
    public class BotConfig
    {
        /// <summary>
        /// Register the bot framwork with Asp.net.
        /// </summary>
        /// <param name="config">Represents the configuration of the HttpServer.</param>
        public static void Register(HttpConfiguration config)
        {
            config.MapBotFramework(botConfig =>
            {
                // Load Connected Services from .bot file
                var path = HostingEnvironment.MapPath(@"~/BasicBot.bot");
                var botConfigurationFile = BotConfiguration.Load(path);
                var service = botConfigurationFile.Services.Where(s => s.Type == "endpoint" && s.Name == "development").FirstOrDefault();
                if (!(service is EndpointService endpointService))
                {
                    throw new InvalidOperationException($"The .bot file does not contain an endpoint with name 'development'.");
                }

                botConfig
                    .UseMicrosoftApplicationIdentity(endpointService?.AppId, endpointService?.AppPassword);

                var connectedServices = InitBotServices(botConfigurationFile);

                UnityConfig.Container.RegisterInstance<BotServices>(connectedServices, new ContainerControlledLifetimeManager());

                // The Memory Storage used here is for local bot debugging only. When the bot
                // is restarted, everything stored in memory will be gone.
                IStorage dataStore = new MemoryStorage();

                // Azure Blob storage.
                // To replace with Azure Blob Storage, add the Microsoft.Bot.Builder.Azure Nuget package to your
                // solution. The package is found at:
                //      https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/
                // IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureBlobStorage("AzureBlobConnectionString", "containerName");

                // Create Conversation State object.
                // The Conversation State object is where we persist anything at the conversation-scope.
                var conversationState = new ConversationState(dataStore);

                // Create User State object.
                // The User State object is where we persist anything at the user-scope.
                var userState = new UserState(dataStore);

                // Create the custom state accessor.
                // State accessors enable other components to read and write individual properties of state.
                var accessors = new BasicBotAccessors(userState, conversationState)
                {
                    GreetingStateProperty = userState.CreateProperty<GreetingState>(BasicBotAccessors.GreetingStateName),
                    DialogStateProperty = conversationState.CreateProperty<DialogState>(BasicBotAccessors.DialogStateName),
                };

                UnityConfig.Container.RegisterInstance<BasicBotAccessors>(accessors, new ContainerControlledLifetimeManager());
            });
        }

        /// <summary>
        /// Initialize the bot's references to external services.
        /// For example, Luis services created here.  External services are configured
        /// using the <see cref="BotConfiguration"/> class (based on the contents of your .bot file).
        /// </summary>
        /// <param name="config">Configuration object based on your .bot file.</param>
        /// <returns>A <see cref="BotConfiguration"/> representing client objects to access external services the bot uses.</returns>
        /// <seealso cref="BotConfiguration"/>
        /// <seealso cref="LuisRecognizer"/>
        /// <seealso cref="TelemetryClient"/>
        private static BotServices InitBotServices(BotConfiguration config)
        {
            var luisServices = new Dictionary<string, LuisRecognizer>();

            foreach (var service in config.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.Luis:
                        {
                            var luis = (LuisService)service;
                            if (luis == null)
                            {
                                throw new InvalidOperationException("The LUIS service is not configured correctly in your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(luis.AppId))
                            {
                                throw new InvalidOperationException("The LUIS Model Application Id ('appId') is required to run this sample. Please update your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(luis.AuthoringKey))
                            {
                                throw new InvalidOperationException("The LUIS Authoring Key ('authoringKey') is required to run this sample. Please update your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(luis.SubscriptionKey))
                            {
                                throw new InvalidOperationException("The Subscription Key ('subscriptionKey') is required to run this sample. Please update your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(luis.Region))
                            {
                                throw new InvalidOperationException("The Region ('region') is required to run this sample.  Please update your '.bot' file.");
                            }

                            var app = new LuisApplication(luis.AppId, luis.SubscriptionKey, luis.Region);
                            var recognizer = new LuisRecognizer(app);
                            luisServices.Add(BasicBot.LuisKey, recognizer);
                            break;
                        }
                }
            }

            var connectedServices = new BotServices(luisServices);
            return connectedServices;
        }
    }
}
