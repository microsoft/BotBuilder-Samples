// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Microsoft.BotBuilderSamples
{
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// The Startup class configures services and the request pipeline.
        /// </summary>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        /// <summary>
        /// Gets the configuration that represents a set of key/value application configuration properties.
        /// </summary>
        /// <value>
        /// The <see cref="IConfiguration"/> that represents a set of key/value application configuration properties.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> specifies the contract for a collection of service descriptors.</param>
        /// <seealso cref="IStatePropertyAccessor{T}"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/dependency-injection"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-channels?view=azure-bot-service-4.0"/>
        public void ConfigureServices(IServiceCollection services)
        {
            // Register the proactive bot.
            services.AddBot<ProactiveBot>(options =>
            {
                InitCredentialProvider(options);

                // Set up error handling. (Trace output goes to the Emulator log, but not to the user.)
                options.OnTurnError = async (context, exception) =>
                {
                    await context.TraceActivityAsync("Proactive bot exception", exception);
                    await context.SendActivityAsync("Sorry, it looks like something went wrong!");
                };

                // The Memory Storage used here is for local bot debugging only. When the bot
                // is restarted, everything stored in memory will be gone.
                IStorage dataStore = new MemoryStorage();

                // Create Job State object.
                // The Job State object is where we persist anything at the job-scope.
                // It's independent of any user or conversation.
                var jobState = new JobState(dataStore);
                options.State.Add(jobState);
            });

            // Validate .bot file endpoint.
            services.AddSingleton(sp =>
            {
                var config = BotConfiguration.Load(@".\ProactiveBot.bot");
                var endpointService = (EndpointService)config.Services.First(s => s.Type == "endpoint")
                    ?? throw new InvalidOperationException(".bot file 'endpoint' must be configured prior to running.");

                return endpointService;
            });

            // Create and register the state accessors for use with this bot.
            // Acessors created here are passed into the IBot-derived class on every turn.
            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value
                    ?? throw new InvalidOperationException(
                        "BotFrameworkOptions must be configured prior to setting up the state accessors.");

                var jobState = options.State.OfType<JobState>().FirstOrDefault();
                if (jobState == null)
                {
                    throw new InvalidOperationException("JobState must be defined and added before adding conversation-scoped state accessors.");
                }

                // Create the custom state accessor.
                // State accessors enable other components to read and write individual properties of state.
                return new ProactiveAccessors(jobState)
                {
                    // Create the state property accessor for job data.
                    JobLogData = jobState.CreateProperty<JobLog>(ProactiveAccessors.JobLogDataName),
                };
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
