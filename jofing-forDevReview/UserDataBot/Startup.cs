namespace UserDataBot
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Azure;
    using Microsoft.Bot.Builder.BotFramework;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Integration;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Builder.TraceExtensions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

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
            // Register your bot
            services.AddBot<UserDataBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);
                options.OnTurnError = async (context, exception) =>
                {
                    await context.TraceActivityAsync("Bot exception", exception);
                    await context.SendActivityAsync("Sorry, it looks like something went wrong!");
                };

                // Add state middleware, with persistent storage.
                var CosmosSettings = Configuration.GetSection("CosmosDB");
                IStorage storage = new CosmosDbStorage(
                    new CosmosDbStorageOptions
                    {
                        AuthKey = CosmosSettings["AuthenticationKey"],
                        CollectionId = CosmosSettings["CollectionID"],
                        CosmosDBEndpoint = new Uri(CosmosSettings["EndpointUri"]),
                        DatabaseId = CosmosSettings["DatabaseID"],
                    });
                var conversationState = new ConversationState(storage);
                var userState = new UserState(storage);
                options.Middleware.Add(new BotStateSet(conversationState, userState));
            });

            // Register the dialog state accessor off of conversation state.
            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                var stateSet = options.Middleware.OfType<BotStateSet>().FirstOrDefault();
                var conversationState = stateSet.BotStates.OfType<ConversationState>().FirstOrDefault();
                return conversationState.CreateProperty<DialogState>("UserDataBot.DialogState");
            });

            // Register the main dialog set.
            services.AddSingleton(sp =>
            {
                var dialogState = sp.GetRequiredService<IStatePropertyAccessor<DialogState>>();
                return new GreetingsDialog(dialogState);
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
