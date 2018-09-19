// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// The Startup class configures services and the app's request pipeline.
    /// </summary>
    public class Startup
    {
        private ILoggerFactory _loggerFactory;

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
        /// <seealso cref="IStatePropertyAccessor{T}"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/dependency-injection"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-channels?view=azure-bot-service-4.0"/>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBot<EchoWithCounterBot>(options =>
            {
                // Load the connected services from .bot file.
                var botConfig = BotConfiguration.Load(@".\BotConfiguration.bot");
                var service = botConfig.Services.FirstOrDefault(s => s.Type == "endpoint");
                var endpointService = service as EndpointService;
                if (endpointService == null)
                {
                    throw new InvalidOperationException("The .bot file does not contain an endpoint.");
                }

                options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

                // Catches any errors that occur during a conversation turn and logs them.
                ILogger logger = _loggerFactory.CreateLogger<EchoWithCounterBot>();
                options.OnTurnError = async (context, exception) =>
                {
                    logger.LogError($"Exception caught : {exception}");
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };

                // The Memory Storage used here is for local bot debugging only. When the bot
                // is restarted, everything stored in memory will be gone.
                IStorage dataStore = new MemoryStorage();

                // For production bots use the Azure Blob or
                // Azure CosmosDB storage providers, as seen below. To the Azure
                // based storage providers, add the Microsoft.Bot.Builder.Azure
                // Nuget package to your solution. That package is found at:
                // https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/
                // Uncomment this line to use Azure Blob Storage
                // IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureBlobStorage("AzureBlobConnectionString", "containerName");

                // Create Conversation State object.
                // The Conversation State object is where we persist anything at the conversation-scope.
                var conversationState = new ConversationState(dataStore);

                options.State.Add(conversationState);
            });

            // Create and register state accesssors.
            // Acessors created here are passed into the IBot-derived class on every turn.
            services.AddSingleton<EchoBotAccessors>(sp =>
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

                // Create the custom state accessor.
                // State accessors enable other components to read and write individual properties of state.
                var accessors = new EchoBotAccessors(conversationState)
                {
                    CounterState = conversationState.CreateProperty<CounterState>(EchoBotAccessors.CounterStateName),
                };

                return accessors;
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
        /// Initializes the credential provider, using by default the <see cref="SimpleCredentialProvider"/>.
        /// </summary>
        /// <param name="options"><see cref="BotFrameworkOptions"/> for the current bot.</param>
        private static void InitCredentialProvider(BotFrameworkOptions options)
        {
            // Load the connected services from .bot file.
            var botConfig = BotConfiguration.Load(@".\BotConfiguration.bot");

            var service = botConfig.Services.FirstOrDefault(s => s.Type == "endpoint");
            var endpointService = service as EndpointService;
            if (endpointService == null)
            {
                throw new InvalidOperationException("The .bot file does not contain an endpoint.");
            }

            options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);
        }
    }
}
