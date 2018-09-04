// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLP_With_Dispatch_Bot.AppInsights;

namespace NLP_With_Dispatch_Bot
{
    /// <summary>
    /// The Startup class configures services and the app's request pipeline.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940.
        /// </summary>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup?view=aspnetcore-2.1"/>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
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
        /// <param name="services">Specifies the contract for a collection of service descriptors.</param>
        /// <seealso cref="IServiceCollection"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/dependency-injection"/>
        public void ConfigureServices(IServiceCollection services)
        {
            // Load the connected services from .bot file
            // BUGBUG: This needs to change to common bot file, once emulator understand appInsights type.
            var botConfig = BotConfiguration.Load(@".\NLP-With-Dispatch-Bot.bot");

            // Initialize Bot Connected Services Clients
            var connectedServices = InitBotServices(botConfig);

            services.AddSingleton(sp => connectedServices);
            services.AddSingleton(sp => botConfig);

            services.AddBot<NlpDispatchBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);

                // Add MyAppInsightsLoggerMiddleware (logs activity messages into Application Insights)
                var appInsightsLogger = new MyAppInsightsLoggerMiddleware(connectedServices.TelemetryClient.InstrumentationKey, logUserName: true, logOriginalMessage: true);
                options.Middleware.Add(appInsightsLogger);
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.  This provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
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
            var luisServices = new Dictionary<string, LuisRecognizer>();

            foreach (var service in config.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.Luis:
                        {
                            // Create a Luis Recognizer that is initialized and suitable for passing
                            // into the IBot-derived class (NlpDispatchBot).
                            // In this case, we're creating a custom class (wrapping the original
                            // Luis Recognizer client) that logs the results of Luis Recognizer results
                            // into Application Insights for future anaysis.
                            var luis = (LuisService)service;
                            if (luis == null)
                            {
                                throw new InvalidOperationException("The Luis service is not configured correctly in your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(luis.AppId))
                            {
                                throw new InvalidOperationException("The Luis Model Application Id ('appId') is required to run this sample.  Please update your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(luis.AuthoringKey))
                            {
                                throw new InvalidOperationException("The Luis Authoring Key ('authoringKey') is required to run this sample.  Please update your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(luis.SubscriptionKey))
                            {
                                throw new InvalidOperationException("The Subscription Key ('subscriptionKey') is required to run this sample.  Please update your '.bot' file.");
                            }

                            if (string.IsNullOrWhiteSpace(luis.Region))
                            {
                                throw new InvalidOperationException("The Region ('region') is required to run this sample.  Please update your '.bot' file.");
                            }

                            var app = new LuisApplication(luis.AppId, luis.AuthoringKey, luis.Region);
                            var recognizer = new MyAppInsightLuisRecognizer(app);
                            luisServices.Add(luis.Name, recognizer);
                            break;
                        }

                    case ServiceTypes.Dispatch:
                        // Create a Dispatch Recognizer that is initialized and suitable for passing
                        // into the IBot-derived class (NlpDispatchBot).
                        // In this case, we're creating a custom class (wrapping the original
                        // Luis Recognizer client) that logs the results of Luis Recognizer results
                        // into Application Insights for future anaysis.
                        var dispatch = (DispatchService)service;
                        if (dispatch == null)
                        {
                            throw new InvalidOperationException("The Luis service is not configured correctly in your '.bot' file.");
                        }

                        if (string.IsNullOrWhiteSpace(dispatch.AppId))
                        {
                            throw new InvalidOperationException("The Luis Model Application Id ('appId') is required to run this sample.  Please update your '.bot' file.");
                        }

                        if (string.IsNullOrWhiteSpace(dispatch.AuthoringKey))
                        {
                            throw new InvalidOperationException("The Luis Authoring Key ('authoringKey') is required to run this sample.  Please update your '.bot' file.");
                        }

                        if (string.IsNullOrWhiteSpace(dispatch.SubscriptionKey))
                        {
                            throw new InvalidOperationException("The Subscription Key ('subscriptionKey') is required to run this sample.  Please update your '.bot' file.");
                        }

                        if (string.IsNullOrWhiteSpace(dispatch.Region))
                        {
                            throw new InvalidOperationException("The Region ('region') is required to run this sample.  Please update your '.bot' file.");
                        }

                        var dispatchApp = new LuisApplication(dispatch.AppId, dispatch.AuthoringKey, dispatch.Region);
                        var dispatchARecognizer = new MyAppInsightLuisRecognizer(dispatchApp);
                        luisServices.Add(dispatch.Name, dispatchARecognizer);
                        break;

                    case ServiceTypes.QnA:
                        {
                            // Create a QnA Maker that is initialized and suitable for passing
                            // into the IBot-derived class (NlpDispatchBot).
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

            return new BotServices(telemetryClient, qnaServices, luisServices);
        }
    }
}
