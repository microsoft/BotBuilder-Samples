// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
using System.Linq;

namespace MessageRoutingBot
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

            this.Configuration = builder.Build();
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
        /// <param name="services">Specifies the contract for a <see cref="IServiceCollection"/> of service descriptors.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
            var botConfig = BotConfiguration.LoadAsync(@".\MessageRouting.bot").GetAwaiter().GetResult();
            services.AddSingleton(sp => botConfig);

            // Initializes your bot service clients and adds a singleton that your Bot can access through dependency injection.
            var connectedServices = InitBotServices(botConfig);
            services.AddSingleton(sp => connectedServices);

            // Initializes Bot Conversation and User State and adds a singleton that your Bot can access through dependency injection.
            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
                var userState = options.State.OfType<UserState>().FirstOrDefault();

                var accessors = new MessageRoutingBotAccessors
                {
                    ConversationDialogState = conversationState.CreateProperty<DialogState>("DialogState"),
                };

                return accessors;
            });

            services.AddBot<MessageRoutingBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(this.Configuration);

                // Catches any errors that occur during a conversation turn and logs them to AppInsights.
                options.OnTurnError = async (context, exception) =>
                {
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };

                // Memory Storage is for local bot debugging only. When the bot
                // is restarted, everything stored in memory will be gone.
                IStorage dataStore = new MemoryStorage();

                // For production bots use the Azure Blob or
                // Azure CosmosDB storage providers, as seen below. To the Azure
                // based storage providers, add the Microsoft.Bot.Builder.Azure
                // Nuget package to your solution. That package is found at:
                // https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/
                // Uncomment this line to use Azure Blob Storage
                // IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureBlobStorage("AzureBlobConnectionString", "containerName");
                // Create and add conversation state.
                var convoState = new ConversationState(dataStore);
                options.State.Add(convoState);
                options.Middleware.Add(new BotStateSet(options.State.ToArray()));
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Application Builder.</param>
        /// <param name="env">Hosting Environment.</param>
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

        /// <summary>
        /// Initializes service clients which will be used throughout the bot code into a single object.
        /// It is recommended that you add any additional service clients you may need into the <see cref="BotServices"/> object and initialize them here.
        /// These services include AppInsights telemetry client, Luis Recognizers, QnAMaker instances, etc.</summary>
        /// <param name="config">Bot configuration object based on .bot json file.</param>
        /// <returns>BotServices object.</returns>
        private BotServices InitBotServices(BotConfiguration config)
        {
            var connectedServices = new BotServices();

            foreach (var service in config.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.Luis:
                        {
                            var luis = service as LuisService;
                            var luisApp = new LuisApplication(luis.AppId, luis.SubscriptionKey, luis.Region);
                            var luisRecognizer = new LuisRecognizer(luisApp);

                            connectedServices.LuisServices.Add(luis.Name, luisRecognizer);
                            break;
                        }
                }
            }

            return connectedServices;
        }
    }
}
