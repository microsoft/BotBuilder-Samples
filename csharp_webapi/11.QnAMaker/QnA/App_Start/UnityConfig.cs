// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace AspNetWebApi_QnA_Bot
{
    using System;
    using System.Web.Hosting;
    using AspNetWebApi_QnA_Bot.AppInsights;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.AI.QnA;
    using Microsoft.Bot.Configuration;
    using Unity;
    using Unity.Lifetime;

    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Gets configured Unity Container.
        /// </summary>
        /// <value>
        /// The Unity container.
        /// </value>
        public static IUnityContainer Container => container.Value;

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IBot, QnABot>();

            // Load Connected Services from .bot file
            var path = HostingEnvironment.MapPath(@"~/AspNetWebApi-Qna.bot");

            var botConfig = BotConfiguration.LoadAsync(path).Result;
            var connectedServices = InitBotServices(botConfig);
            container.RegisterInstance<BotServices>(connectedServices, new ContainerControlledLifetimeManager());
        }

        /// <summary>
        /// Initializes the Azure Services used in this Bot.
        /// These will be held in a singleton-level scope and used
        /// in processing Activity messages.
        /// </summary>
        /// <param name="config">The BotConfiguration for this Bot.
        /// See BotConfiguration for more information.</param>
        /// <returns>A populated BotServices class to be used within the Bot.</returns>
        private static BotServices InitBotServices(BotConfiguration config)
        {
            var connectedServices = new BotServices();

            foreach (var service in config.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.QnA:
                        {
                            var qna = service as QnAMakerService;
                            var qnaEndpoint = new QnAMakerEndpoint()
                            {
                                KnowledgeBaseId = qna.KbId,
                                EndpointKey = qna.EndpointKey,
                                Host = qna.Hostname,
                            };
                            var qnaMaker = new MyAppInsightsQnaMaker(qnaEndpoint);
                            connectedServices.QnAServices.Add(qna.Name, qnaMaker);

                            break;
                        }

                    case ServiceTypes.AppInsights:
                        {
                            var appInsights = service as AppInsightsService;
                            var telemetryConfig = new TelemetryConfiguration(appInsights.InstrumentationKey);
                            connectedServices.TelemetryClient = new TelemetryClient(telemetryConfig);
                            break;
                        }
                }
            }

            return connectedServices;
        }
    }
}
