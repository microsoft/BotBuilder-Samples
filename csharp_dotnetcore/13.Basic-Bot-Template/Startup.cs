// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.BotBuilderSamples
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Specifies the contract for a <see cref="IServiceCollection"/> of service descriptors.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
        public void ConfigureServices(IServiceCollection services)
        {
            // Load the connected services from .bot file.
            var botConfig = BotConfiguration.Load(@".\BasicBot.bot");

            // Initialize Bot Connected Services clients.
            var connectedServices = InitBotServices(botConfig);
            services.AddSingleton(sp => connectedServices);

            services.AddSingleton(sp => botConfig);

            services.AddBot<BasicBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);

                // The Memory Storage used here is for local bot debugging only. When the bot
                // is restarted, everything stored in memory will be gone.
                IStorage dataStore = new MemoryStorage();

                // Create Conversation State object.
                // The Conversation State object is where we persist anything at the conversation-scope.
                var conversationState = new ConversationState(dataStore);
                options.State.Add(conversationState);

                // Create User State object.
                // The User State object is where we persist anything at the user-scope.
                var userState = new UserState(dataStore);
                options.State.Add(userState);
            });

            // Create and register state accesssors.
            // Acessors created here are passed into the IBot-derived class on every turn.
            services.AddSingleton<BasicBotAccessors>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                if (options == null)
                {
                    throw new InvalidOperationException("BotFrameworkOptions must be configured prior to setting up the state accessors");
                }

                var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
                if (conversationState == null)
                {
                    throw new InvalidOperationException("ConversationState must be defined and added before adding conversation-scoped state accessors.");
                }

                var userState = options.State.OfType<UserState>().FirstOrDefault();
                if (userState == null)
                {
                    throw new InvalidOperationException("UserState must be defined and added before adding usre-scoped state accessors.");
                }

                // Create the custom state accessor.
                // State accessors enable other components to read and write individual properties of state.
                var accessors = new BasicBotAccessors(userState, conversationState)
                {
                    GreetingStateProperty = userState.CreateProperty<GreetingState>(BasicBotAccessors.GreetingStateName),
                    DialogStateProperty = conversationState.CreateProperty<DialogState>(BasicBotAccessors.DialogStateName),
                };

                return accessors;
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder. This provides the mechanisms to configure the application request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }

        /// <summary>
        /// Initialize the bot's references to external services.
        ///
        /// For example, LUIS services are created here. This external services is configured
        /// using the <see cref="BotConfiguration"/> class (based on the contents of your ".bot" file).
        /// </summary>
        /// <param name="config">The <see cref="BotConfiguration"/> object based on your ".bot" file.</param>
        /// <returns>A <see cref="BotServices"/> representing client objects to access external services the bot uses.</returns>
        /// <seealso cref="BotConfiguration"/>
        /// <seealso cref="LuisRecognizer"/>
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
                                throw new InvalidOperationException("The Region ('region') is required to run this sample. Please update your '.bot' file.");
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
