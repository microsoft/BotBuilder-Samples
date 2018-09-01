// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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

namespace AspNetWebApi_QnA_Bot
{
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
        /// Registers the Bot and services configuration (based on the .bot file).
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
        /// Initialize the bot's references to external services.
        ///
        /// For example, Application Insights and QnaMaker services
        /// are created here.  These external services are configured
        /// using the BotConfigure class (based on the contents of your ".bot" file).
        /// </summary>
        /// <param name="config">Configuration object based on your ".bot" file.</param>
        /// <returns>A object representing client objects to access external services the bot uses.</returns>
        private static BotServices InitBotServices(BotConfiguration config)
        {
            var connectedServices = new BotServices();

            foreach (var service in config.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.QnA:
                        {
                            // Create a QNA Maker that is initialized and suitable for passing
                            // into the IBot-derived class (QnABot).
                            // In this case, we're creating a custom class (wrapping the original
                            // QnAMaker client) that logs the results of QnA Maker into Application
                            // Insights for future anaysis.
                            var qna = service as QnAMakerService;
                            if (string.IsNullOrWhiteSpace(qna.KbId))
                            {
                                throw new InvalidOperationException("The Qna KnowledgeBaseId ('kbId') is required to run this sample.  Please update your appsettings.json for more details.");
                            }

                            if (string.IsNullOrWhiteSpace(qna.EndpointKey))
                            {
                                throw new InvalidOperationException("The Qna EndpointKey ('endpointKey') is required to run this sample.  Please update your QnaBot.bot file.");
                            }

                            if (string.IsNullOrWhiteSpace(qna.Hostname))
                            {
                                throw new InvalidOperationException("The Qna Host ('hostname') is required to run this sample.  Please update your QnaBot.bot file.");
                            }

                            var qnaEndpoint = new QnAMakerEndpoint()
                            {
                                KnowledgeBaseId = qna.KbId,
                                EndpointKey = qna.EndpointKey,
                                Host = qna.Hostname,
                            };

                            var qnaMaker = new MyAppInsightsQnaMaker(qnaEndpoint, null, logUserName: false, logOriginalMessage: false);
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
