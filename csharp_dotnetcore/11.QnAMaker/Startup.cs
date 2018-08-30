// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspNetCore_QnA_Bot.AppInsights;

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
            services.AddBot<QnABot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);

                // Add MyAppInsightsLoggerMiddleware (logs activity messages into Application Insights)
                var instrumentationKey = Configuration.GetSection("ApplicationInsights")?.GetSection("InstrumentationKey").Value;
                if (null == instrumentationKey)
                {
                    throw new InvalidOperationException("Application Insights instrumentation key must be configured in the appsettings.json file.");
                }
                var appInsightsLogger = new MyAppInsightsLoggerMiddleware(instrumentationKey, logUserName: true, logOriginalMessage: true);
                options.Middleware.Add(appInsightsLogger);
            });

            // Create a QNA Maker that is initialized and suitable for passing
            // into the IBot-derived class (QnABot). 
            // This custom class also logs results to Application Insights.
            services.AddSingleton<MyAppInsightsQnaMaker>(sp =>
            {
                var knowledgeBaseId = Configuration.GetSection("QnaMaker-KnowledgeBaseId")?.Value;
                var endpointKey = Configuration.GetSection("QnaMaker-EndpointKey")?.Value;
                var host = Configuration.GetSection("QnaMaker-Host")?.Value;

                if (string.IsNullOrWhiteSpace(knowledgeBaseId))
                {
                    throw new InvalidOperationException("The Qna KnowledgeBaseId ('QnaMaker-KnowledgeBaseId') is required to run this sample.  Please update your appsettings.json for more details.");
                }

                if (string.IsNullOrWhiteSpace(endpointKey))
                {
                    throw new InvalidOperationException("The Qna EndpointKey ('QnaMaker-EndpointKey') is required to run this sample.  Please update your appsettings.json for more details.");
                }

                if (string.IsNullOrWhiteSpace(host))
                {
                    throw new InvalidOperationException("The Qna Host ('QnaMaker-Host') is required to run this sample.  Please update your appsettings.json for more details.");
                }

                var qnaEndpoint = new QnAMakerEndpoint()
                {
                    KnowledgeBaseId = knowledgeBaseId,
                    EndpointKey = endpointKey,
                    Host = host
                };

                var qnaOptions = new QnAMakerOptions()
                {
                    ScoreThreshold = 0.3f
                };

                var myQnaRecognizer = new MyAppInsightsQnaMaker(qnaEndpoint, qnaOptions, logUserName: true, logOriginalMessage: true);
                return myQnaRecognizer;
            });
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
