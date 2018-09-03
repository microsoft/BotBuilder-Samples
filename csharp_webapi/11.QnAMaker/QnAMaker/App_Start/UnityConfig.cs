// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Web.Hosting;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Configuration;
using QnA_Bot.AppInsights;
using Unity;
using Unity.Lifetime;

namespace QnA_Bot
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/dependency-injection"/>
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
        /// The <see cref="IUnityContainer"/> container.
        /// </value>
        public static IUnityContainer Container => container.Value;

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The <see cref="IUnityContainer"/> container to configure.</param>
        /// <remarks>
        /// Registers the <see cref="IBot"/> and services <see cref="BotConfiguration"/> (based on the .bot file).
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IBot, QnABot>();

            // Load Connected Services from .bot file
            var path = HostingEnvironment.MapPath(@"~/QnABot.bot");

            var botConfig = BotConfiguration.Load(path);
            var connectedServices = InitBotServices(botConfig);
            container.RegisterInstance<BotServices>(connectedServices, new ContainerControlledLifetimeManager());
        }

        /// <summary>
        /// Initialize the bot's references to external services.
        ///
        /// For example, Application Insights and QnaMaker services
        /// are created here.  These external services are configured
        /// using the <see cref="BotConfiguration"/> class (based on the contents of your ".bot" file).
        /// </summary>
        /// <param name="config">Configuration object based on your ".bot" file.</param>
        /// <returns>A <see cref="BotConfiguration"/> representing client objects to access external services the bot uses.</returns>
        /// <seealso cref="BotConfiguration"/>
        /// <seealso cref="QnAMaker"/>
        /// <seealso cref="TelemetryClient"/>
        private static BotServices InitBotServices(BotConfiguration config)
        {
            TelemetryClient telemetryClient = null;
            var qnaServices = new Dictionary<string, QnAMaker>();

            foreach (var service in config.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.QnA:
                        {
                            // Create a QnA Maker that is initialized and suitable for passing
                            // into the IBot-derived class (QnABot).
                            // In this case, we're creating a custom class (wrapping the original
                            // QnAMaker client) that logs the results of QnA Maker into Application
                            // Insights for future anaysis.
                            var qna = (QnAMakerService)service;
                            if (qna == null)
                            {
                                throw new InvalidOperationException("The QnA service is not configured correctly in your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(qna.KbId))
                            {
                                throw new InvalidOperationException("The Qna KnowledgeBaseId ('kbId') is required to run this sample.  Please update your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(qna.EndpointKey))
                            {
                                throw new InvalidOperationException("The Qna EndpointKey ('endpointKey') is required to run this sample.  Please update your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(qna.Hostname))
                            {
                                throw new InvalidOperationException("The Qna Host ('hostname') is required to run this sample.  Please update your '.bot' file.");
                            }

                            var qnaEndpoint = new QnAMakerEndpoint()
                            {
                                KnowledgeBaseId = qna.KbId,
                                EndpointKey = qna.EndpointKey,
                                Host = qna.Hostname,
                            };

                            var qnaMaker = new MyAppInsightsQnAMaker(qnaEndpoint, null, logUserName: false, logOriginalMessage: false);
                            qnaServices.Add(qna.Name, qnaMaker);

                            break;
                        }

                    case ServiceTypes.AppInsights:
                        {
                            var appInsights = (AppInsightsService)service;
                            if (appInsights == null)
                            {
                                throw new InvalidOperationException("The Application Insights is not configured correctly in your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(appInsights.InstrumentationKey))
                            {
                                throw new InvalidOperationException("The Application Insights Instrumentation Key ('instrumentationKey') is required to run this sample.  Please update your '.bot' file.");
                            }

                            var telemetryConfig = new TelemetryConfiguration(appInsights.InstrumentationKey);
                            telemetryClient = new TelemetryClient(telemetryConfig);
                            telemetryClient.InstrumentationKey = appInsights.InstrumentationKey;
                            break;
                        }
                }
            }

            var connectedServices = new BotServices(telemetryClient, qnaServices);
            return connectedServices;
        }
    }
}
