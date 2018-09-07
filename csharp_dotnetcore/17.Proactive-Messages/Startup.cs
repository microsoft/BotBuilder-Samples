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

        public void ConfigureServices(IServiceCollection services)
        {
            // Register the proactive bot.
            services.AddBot<ProactiveBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);

                // Set up error handling. (Trace output goes to the Emulator log, but not to the user.)
                options.OnTurnError = async (context, exception) =>
                {
                    await context.TraceActivityAsync("Proactive bot exception", exception);
                    await context.SendActivityAsync("Sorry, it looks like something went wrong!");
                };

                // Set up state management middleware.
                IStorage dataStore = new MemoryStorage();
                var state = new JobState(dataStore);
                options.Middleware.Add(state);
            });

            // Validate .bot file endpoint
            services.AddSingleton(sp =>
            {
                var config = BotConfiguration.Load(@".\ProactiveBot.bot");
                var endpointService = (EndpointService)config.Services.First(s => s.Type == "endpoint")
                    ?? throw new InvalidOperationException(".bot file 'endpoint' must be configured prior to running.");
                if (string.IsNullOrWhiteSpace(endpointService.AppId))
                {
                    throw new InvalidOperationException(".bot file 'endpoint' must contain application id (appId) prior to running.");
                }

                return endpointService;
            });

            // Create and register the state accessors for use with this bot.
            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value
                    ?? throw new InvalidOperationException(
                        "BotFrameworkOptions must be configured prior to setting up the state accessors.");

                var jobState = options.Middleware.OfType<JobState>().FirstOrDefault()
                    ?? throw new InvalidOperationException(
                        "Job state must be defined and added before adding job-scoped state accessors.");

                return new ProactiveAccessors
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
    }
}
