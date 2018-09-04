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
using WelcomeUser.State;

namespace WelcomeUser
{
    /// <summary>
    /// The Startup class configures services and the app's request pipeline.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Gets the configuration that represents a set of key/value application configuration properties.
        /// </summary>
        /// <value>
        /// The IConfiguration that represents a set of key/value application configuration properties.
        /// </value>
        public IConfiguration Configuration { get; }


        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// ASP.NET Core Application Statup code. This method gets called by the runtime. 
        /// Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="env">Provides information about the web hosting environment an application is running in </param>
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
        /// This method gets called by ASP.NET Core runtime as part of initializaing your application
        /// Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddBot<WelcomeUserBot>(options =>
                {
                    options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);

                    // The Memory Storage used here is for local bot debugging only. When the bot
                    // is restarted, anything stored in memory will be gone. 

                    IStorage dataStore = new MemoryStorage();

                    // For production bots use Azure Blob, or Azure CosmosDB storage provides
                    // as seen below. To include any of the Azure based storage providers, 
                    // add the Microsoft.Bot.Builder.Azure  Nuget package to your solution. That package is found at:
                    //      https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/

                    // IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureBlobStorage("AzureBlobConnectionString", "containerName");

                    //var convoState = new ConversationState(dataStore);
                    //options.State.Add(new ConversationState(dataStore));
                    options.State.Add(new UserState(dataStore));

                    // Add State to BotStateSet Middleware (that require auto-save)
                    // The BotStateSet Middleware forces state storage to auto-save when the Bot is complete processing the message.
                    // Note: Developers may choose not to add all the State providers to this Middleware if save is not required.
                    //var stateSet = new BotStateSet(options.State.ToArray());
                    options.Middleware.Add(new BotStateSet(options.State.ToArray()));
                    
                });

                // register 'object' (classes) into the runtime services collection
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
                        throw new InvalidOperationException("ConversationState must be defined and added before adding conversation-scoped state accessors.");
                    }

                    // Create Custom State Property Accessors
                    // State Property Accessors enable components to read and write individual properties, without having to 
                    // pass the entire State object.
                    var accessors = new WelcomeUserStateAccessors
                    {
                        //TODO: change static string and put in accessor class
                        DidBotWelcomedUser = userState.CreateProperty<Boolean>("DidBotWelcomeState") 
                    };

                    return accessors;
                });



            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.  This provides the mechanisms to configure an application's request pipeline.</param>
        /// <param name="env">Provides information about the web hosting environment an application is running in.</param>
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
