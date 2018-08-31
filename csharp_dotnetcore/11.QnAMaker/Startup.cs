// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using AspNetCore_QnA_Bot.AppInsights;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCore_QnA_Bot
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Load Connected Services from .bot file
            // BUGBUG: This needs to change to common bot file, once emulator understand appInsights type.
            var botConfig = BotConfiguration.LoadAsync(@".\QnaBot.bot").Result;

            // Initialize Bot Connected Services Clients
            var connectedServices = InitBotServices(botConfig);
            services.AddSingleton(sp => connectedServices);

            services.AddSingleton(sp => botConfig);

            services.AddBot<QnABot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);

                // Add MyAppInsightsLoggerMiddleware (logs activity messages into Application Insights)
                var appInsightsLogger = new MyAppInsightsLoggerMiddleware(connectedServices.TelemetryClient.InstrumentationKey, logUserName: true, logOriginalMessage: true);
                options.Middleware.Add(appInsightsLogger);
            });
            

        }

        // Configure Bot Services
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
                            // This custom class also logs results to Application Insights.
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
                            var qnaOptions = new QnAMakerOptions()
                            {
                                ScoreThreshold = 0.3f
                            };

                            var qnaMaker = new MyAppInsightsQnaMaker(qnaEndpoint, qnaOptions, logUserName: false, logOriginalMessage: false);
                            connectedServices.QnAServices.Add(qna.Name, qnaMaker);

                            break;
                        }

                    case ServiceTypes.AppInsights:
                        {
                            var appInsights = service as AppInsightsService;
                            if (string.IsNullOrWhiteSpace(appInsights.InstrumentationKey))
                            {
                                throw new InvalidOperationException("The Application Insights Instrumentation Key ('instrumentationKey') is required to run this sample.  Please update your QnaBot.bot file.");
                            }

                            var telemetryConfig = new TelemetryConfiguration(appInsights.InstrumentationKey);
                            connectedServices.TelemetryClient = new TelemetryClient(telemetryConfig);
                            connectedServices.TelemetryClient.InstrumentationKey = appInsights.InstrumentationKey;
                            break;
                        }
                }
            }
            return connectedServices;
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }
    }
}
