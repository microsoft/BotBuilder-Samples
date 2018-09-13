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
using Microsoft.Extensions.Options;
using WelcomeUser.State;

namespace WelcomeUser
{
    /// <summary>
    /// The Startup class configures services and the app's request pipeline.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// This method is part of ASP.NET Core Application Statup code and gets called by the runtime.
        /// Use this method to add services to the container.
        /// For more information on how to configure your application, visit <see ref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup"/>.
        /// </summary>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        /// <summary>
        /// Gets the configuration object.
        /// </summary>
        /// <value>
        /// The IConfiguration that represents a set of key/value application configuration properties.
        /// </value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by ASP.NET Core runtime as part of initializaing your application
        /// Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBot<WelcomeUserBot>(options =>
            {
                // Load the connected services from .bot file.
                var botConfig = BotConfiguration.Load(@".\WelcomeUser.bot");

                var service = botConfig.Services.FirstOrDefault(s => s.Type == "endpoint");
                var endpointService = service as EndpointService;
                if (endpointService == null)
                {
                    throw new InvalidOperationException("The .bot file does not contain an endpoint.");
                }

                options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

                // The Memory Storage used here is for local bot debugging only. When the bot
                // is restarted, anything stored in memory will be gone.
                IStorage dataStore = new MemoryStorage();

                // For production bots use Azure Blob, or Azure CosmosDB storage provides
                // as seen below. To include any of the Azure based storage providers,
                // add the Microsoft.Bot.Builder.Azure  Nuget package to your solution. That package is found at:
                // https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/
                // IStorage Store = new Microsoft.Bot.Builder.Azure.AzureBlobStorage("AzureBlobConnectionString", "containerName");

                // var convoState = new ConversationState(dataStore);
                // options.State.Add(new ConversationState(dataStore));
                options.State.Add(new UserState(dataStore));
            });

            services.AddSingleton<WelcomeUserStateAccessors>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                if (options == null)
                {
                    throw new InvalidOperationException("BotFrameworkOptions must be configured prior to setting up the State Accessors");
                }

                var userState = options.State.OfType<UserState>().FirstOrDefault();
                if (userState == null)
                {
                    throw new InvalidOperationException("UserState must be defined and added before adding user-scoped state accessors.");
                }

                // Create custom state property accessors
                // State property accessors enable components to read and write individual properties,
                // without having to pass the entire State object.
                var accessors = new WelcomeUserStateAccessors(userState)
                {
                    DidBotWelcomedUser = userState.CreateProperty<bool>("DidBotWelcomeState"),
                };

                return accessors;
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder. This provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }
    }
}
