// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Integration.AspNet.WebApi;
using Microsoft.Bot.Configuration;
using Unity;
using Unity.Lifetime;

namespace LuisBot
{
    /// <summary>
    /// Performs the Bot-specific configuration during Asp.Net start.
    /// </summary>
    public class BotConfig
    {
        /// <summary>
        /// Register the bot framework with Asp.net.
        /// </summary>
        /// <param name="config">Represents the configuration of the HttpServer.</param>
        public static void Register(HttpConfiguration config)
        {
            config.MapBotFramework(botConfig =>
            {
                // Load Connected Services from .bot file
                var path = HostingEnvironment.MapPath(@"~/nlp-with-luis.bot");
                var botConfigurationFile = BotConfiguration.Load(path);
                var endpointService = (EndpointService)botConfigurationFile.Services.First(s => s.Type == "endpoint");

                botConfig
                    .UseMicrosoftApplicationIdentity(endpointService?.AppId, endpointService?.AppPassword);

                var connectedServices = InitBotServices(botConfigurationFile);

                UnityConfig.Container.RegisterInstance<BotServices>(connectedServices, new ContainerControlledLifetimeManager());
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

                            // CAUTION: Authoring key is used in this example as it is appropriate for prototyping.
                            // When implimenting for deployment/production, assign and use a subscription key instead of an authoring key.
                            var app = new LuisApplication(luis.AppId, luis.AuthoringKey, luis.GetEndpoint());
                            var recognizer = new LuisRecognizer(app);
                            luisServices.Add(LuisBot.LuisKey, recognizer);
                            break;
                        }
                }
            }

            var connectedServices = new BotServices(luisServices);
            return connectedServices;
        }
    }
}
