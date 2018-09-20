// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
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
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/dependency-injection"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-channels?view=azure-bot-service-4.0"/>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBot<WelcomeUserBot>(options =>
            {
                InitCredentialProvider(options);

                // The Memory Storage used here is for local bot debugging only. When the bot
                // is restarted, anything stored in memory will be gone.
                IStorage dataStore = new MemoryStorage();

                // For production bots use Azure Blob, or Azure CosmosDB storage provides
                // as seen below. To include any of the Azure based storage providers,
                // add the Microsoft.Bot.Builder.Azure  Nuget package to your solution. That package is found at:
                // https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/
                // Uncomment this line to use Azure Blob Storage
                // IStorage Store = new Microsoft.Bot.Builder.Azure.AzureBlobStorage("AzureBlobConnectionString", "containerName");

                // Create User State object.
                // The User State object is where we persist anything at the user-scope.
                var userState = new UserState(dataStore);
                options.State.Add(userState);
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

                // Create the custom state accessor.
                // State accessors enable other components to read and write individual properties of state.
                var accessors = new WelcomeUserStateAccessors(userState)
                {
                    DidBotWelcomedUser = userState.CreateProperty<bool>("DidBotWelcomeState"),
                };

                return accessors;
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
