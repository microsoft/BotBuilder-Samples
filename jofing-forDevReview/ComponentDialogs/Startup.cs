namespace ComponentDialogs
{
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Bot.Builder;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBot<HotelBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);
                options.OnTurnError = async (context, exception) =>
                {
                    await context.TraceActivityAsync("Bot Exception", exception);
                    await context.SendActivityAsync("Sorry, it looks like something went wrong!");
                };

                // We're using both conversation and user state.
                IStorage dataStore = new MemoryStorage();
                var convState = new ConversationState(dataStore);
                var userState = new UserState(dataStore);
                options.Middleware.Add(new BotStateSet(convState, userState));
            });

            // Create and register the dialog state accessor off of conversation state.
            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                var stateSet = options.Middleware.OfType<BotStateSet>().FirstOrDefault();
                var convState = stateSet.BotStates.OfType<ConversationState>().FirstOrDefault();
                return convState.CreateProperty<DialogState>("IntegratedDialogs.DialogState");
            });

            // Create and register the user profile state accessor off of user state.
            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                var stateSet = options.Middleware.OfType<BotStateSet>().FirstOrDefault();
                var userState = stateSet.BotStates.OfType<UserState>().FirstOrDefault();
                return userState.CreateProperty<UserProfile>("IntegratedDialogs.UserProfile");
            });

            // Create and register the main dialog set.
            services.AddSingleton(sp =>
            {
                var dialogState = sp.GetRequiredService<IStatePropertyAccessor<DialogState>>();
                var userProfileAccessor = sp.GetRequiredService<IStatePropertyAccessor<UserProfile>>();
                return new MainDialog(dialogState, userProfileAccessor);
            });
        }
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
