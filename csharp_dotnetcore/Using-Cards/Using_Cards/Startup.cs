// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Using_Cards
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

            this.Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddBot<CardsBot>(options =>
                {
                    options.CredentialProvider = new ConfigurationCredentialProvider(this.Configuration);

                    // The Memory Storage used here is for local bot debugging only. When the bot
                    // is restarted, anything stored in memory will be gone.
                    IStorage dataStore = new MemoryStorage();

                    // For production bots use the Azure Blob or
                    // Azure CosmosDB storage provides, as seen below. To include any of
                    // the Azure based storage providers, add the Microsoft.Bot.Builder.Azure
                    // Nuget package to your solution. That package is found at:
                    // https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/
                    // Uncomment this lone to use blob storage
                    // IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureBlobStorage("AzureBlobConnectionString", "containerName");

                    var convoState = new ConversationState(dataStore);
                    options.State.Add(convoState);

                    // Add state to BotStateSet middleware (that require auto-save).
                    // The BotStateSet middleware forces state storage to auto-save when the bot is complete processing the message.
                    // Note: Developers may choose not to add all the state providers to this middleware if save is not required.
                    var stateSet = new BotStateSet(options.State.ToArray());
                    options.Middleware.Add(stateSet);
                });

                services.AddSingleton<CardsBotAccessors>(sp =>
                {
                    var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                    if (options == null)
                    {
                        throw new InvalidOperationException(
                            "BotFrameworkOptions must be configured prior to setting up the State Accessors");
                    }

                    var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
                    if (conversationState == null)
                    {
                        throw new InvalidOperationException(
                            "ConversationState must be defined and added before adding conversation-scoped state accessors.");
                    }

                    // Create custom state property accessors.
                    // State property accessors enable components to read and write individual properties, without having to
                    // pass the entire state object.
                    var accessors = new CardsBotAccessors
                    {
                        ConversationDialogState =
                            conversationState.CreateProperty<DialogState>(CardsBotAccessors.DialogStateName),
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
