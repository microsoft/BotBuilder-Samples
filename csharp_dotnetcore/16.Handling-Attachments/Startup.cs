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
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Handling_Attachments
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();

            // This needs to be set to access a filepath in the bot.
            this.Environment = env;
            this.Configuration = builder.Build();
        }

        /// <summary>
        /// Gets the <see cref="IConfiguration"/> that represents a set of key/value application configuration properties.
        /// </summary>
        /// <value>
        /// The <see cref="IConfiguration"/> that represents a set of key/value application configuration properties.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets the <see cref="IHostingEnvironment"/> for the current environment of the bot.
        /// This is used to get the correct filepath to send and receive attachments in the bot.
        /// </summary>
        /// <value>
        /// The <see cref="IHostingEnvironment"/> that provides information about the web hosting environment an application is running in.
        /// </value>
        public IHostingEnvironment Environment { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Specifies the contract for a <see cref="IServiceCollection"/> of service descriptors.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBot<AttachmentsBot>(options =>
            {
                InitCredentialProvider(options);
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>. This provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Provides information about the <see cref="IHostingEnvironment"/> an application is running in.</param>
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
