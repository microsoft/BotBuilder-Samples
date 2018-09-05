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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EchoBotWithCounter
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
        /// <param name="services">The <see cref="IServiceCollection"/> specifies the contract for a collection of service descriptors.</param>
        /// <seealso cref="IStatePropertyAccessor{T}"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/dependency-injection"/>
        /// <seealso cref="https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-channels?view=azure-bot-service-4.0"/>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBot<EchoWithCounterBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);

                // The Memory Storage used here is for local bot debugging only. When the bot
                // is restarted, anything stored in memory will be gone.
                IStorage dataStore = new MemoryStorage();

                // Create Conversation State object.
                // The Conversation State object is where we persist anything at the conversation-scope (note: the definition of a
                // conversation is channel specific.  Example of channels are "skype" or "slack".).
                // The User and Conversation state objects are very commonly used.  Custom state objects can also be
                // created.  Again, the behavior is channel-specific.
                //
                // NOTE: State Property Accessors that are required for Middleware components *could* be built here
                // for passing into Middleware construction below.
                // In this particular sample, there are no components that are passed state property accessors in the
                // Startup.ConfigureServices() method.
                // However, all state property accessors are built and passed to the IBot-derived class via Asp.net Direct
                // Injection via the Singleton defined below.
                var conversationState = new ConversationState(dataStore);

                // Add to State Object to options State list.
                // Objects in the State list enable other components to get access to the state providers
                // during the start up process.  For example, creating state property accessors within a ASP.net Core Singleton
                // that could be passed to your IBot-derived class.
                // Note: The providers in this list are not associated with the BotStateSet Middleware component. To clarify,
                // state providers in this list are not automatically loaded or saved during the turn process.
                options.State.Add(conversationState);

                // Add State to BotStateSet Middleware.
                // The BotStateSet Middleware forces the state storage to save when the Bot is complete processing the message.
                // Note: You may choose not to add all the State providers to this Middleware if save is not required or
                // saving the state is handled in your code.
                var stateSet = new BotStateSet(options.State.ToArray());
                options.Middleware.Add(stateSet);
            });

            // Create and register any state accesssors.
            // The accessor created here is passed into the IBot-derived class on every turn.
            services.AddSingleton<EchoBotAccessors>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                if (options == null)
                {
                    throw new InvalidOperationException("BotFrameworkOptions must be configured prior to setting up the State Accessors");
                }

                var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
                if (conversationState == null)
                {
                    throw new InvalidOperationException("ConversationState must be defined and added before adding conversation-scoped state accessors.");
                }

                // Create Custom State Property Accessor.
                // State Property Accessors enable components to read and write individual properties, without having to
                // pass the entire State object.
                var accessors = new EchoBotAccessors
                {
                    CounterState = conversationState.CreateProperty<CounterState>(EchoBotAccessors.CounterName),
                };

                return accessors;
            });
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">The <see cref="IHostingEnvironment"/> provides information about the web hosting environment an application is running in.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }
    }
}
