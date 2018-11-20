// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Web.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Configuration;
using Unity;
using Unity.Lifetime;

namespace QnABot
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
        /// For example, QnaMaker services
        /// is created here.  These external services are configured
        /// using the <see cref="BotConfiguration"/> class (based on the contents of your ".bot" file).
        /// </summary>
        /// <param name="config">Configuration object based on your ".bot" file.</param>
        /// <returns>A <see cref="BotConfiguration"/> representing client objects to access external services the bot uses.</returns>
        /// <seealso cref="BotConfiguration"/>
        /// <seealso cref="QnAMaker"/>
        /// <seealso cref="TelemetryClient"/>
        private static BotServices InitBotServices(BotConfiguration config)
        {
            var qnaServices = new Dictionary<string, QnAMaker>();

            foreach (var service in config.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.QnA:
                        {
                            var qna = (QnAMakerService)service;
                            if (qna == null)
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

            var connectedServices = new BotServices(qnaServices);
            return connectedServices;
        }
    }
}
