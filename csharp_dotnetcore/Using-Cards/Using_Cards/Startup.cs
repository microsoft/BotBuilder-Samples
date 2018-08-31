using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Extensions.Options;

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

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddBot<CardsBot>(options =>
                {
                    options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);

                    // The CatchExceptionMiddleware provides a top-level exception handler for your bot. 
                    // Any exceptions thrown by other Middleware, or by your OnTurn method, will be 
                    // caught here. To facillitate debugging, the exception is sent out, via Trace, 
                    // to the emulator. Trace activities are NOT displayed to users, so in addition
                    // an "Ooops" message is sent. 


                    // The Memory Storage used here is for local bot debugging only. When the bot
                    // is restarted, anything stored in memory will be gone. 

                    IStorage dataStore = new MemoryStorage();

                    // The File data store, shown here, is suitable for bots that run on 
                    // a single machine and need durable state across application restarts.                 
                    // IStorage dataStore = new FileStorage(System.IO.Path.GetTempPath());

                    // For production bots use the Azure Table Store, Azure Blob, or 
                    // Azure CosmosDB storage provides, as seen below. To include any of 
                    // the Azure based storage providers, add the Microsoft.Bot.Builder.Azure 
                    // Nuget package to your solution. That package is found at:
                    //      https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/

                    // IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureTableStorage("AzureTablesConnectionString", "TableName");
                    // IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureBlobStorage("AzureBlobConnectionString", "containerName");

                    var convoState = new ConversationState(dataStore);
                    options.State.Add(convoState);

                    // Add State to BotStateSet Middleware (that require auto-save)
                    // The BotStateSet Middleware forces state storage to auto-save when the Bot is complete processing the message.
                    // Note: Developers may choose not to add all the State providers to this Middleware if save is not required.
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

                    // Create Custom State Property Accessors
                    // State Property Accessors enable components to read and write individual properties, without having to 
                    // pass the entire State object.
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
