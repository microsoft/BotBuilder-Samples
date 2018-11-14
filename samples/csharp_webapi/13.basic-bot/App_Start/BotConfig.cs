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
        /// Register the bot framework with Asp.net.
        /// </summary>
        /// <param name="config">Represents the configuration of the HttpServer.</param>
        public static void Register(HttpConfiguration config)
        {
            config.MapBotFramework(botConfig =>
            {
                // Load Connected Services from .bot file
                var path = HostingEnvironment.MapPath(@"~/basic-bot.bot");
                var botConfigurationFile = BotConfiguration.Load(path);
                var service = botConfigurationFile.Services.Where(s => s.Type == "endpoint" && s.Name == "development").FirstOrDefault();
                if (!(service is EndpointService endpointService))
                {
                    throw new InvalidOperationException($"The .bot file does not contain an endpoint with name 'development'.");
                }

                botConfig
                    .UseMicrosoftApplicationIdentity(endpointService?.AppId, endpointService?.AppPassword);

                var connectedServices = new BotServices(botConfigurationFile);

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

                UnityConfig.Container.RegisterInstance<ConversationState>(conversationState, new ContainerControlledLifetimeManager());
                UnityConfig.Container.RegisterInstance<UserState>(userState, new ContainerControlledLifetimeManager());

                // The BotStateSet enables read() and write() in parallel on multiple BotState instances.
                UnityConfig.Container.RegisterInstance<BotStateSet>(new BotStateSet(userState, conversationState), new ContainerControlledLifetimeManager());

                // Automatically save state at the end of a turn.
                botConfig.BotFrameworkOptions.Middleware
                    .Add(new AutoSaveStateMiddleware(userState, conversationState));
            });
        }
    }
}
