// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using LivePersonConnector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LivePersonProxyBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var storage = new MemoryStorage();
            // Create the User state passing in the storage layer.
            var userState = new UserState(storage);
            services.AddSingleton(userState);

            // Create the Conversation state passing in the storage layer.
            var conversationState = new ConversationState(storage);
            services.AddSingleton(conversationState);

            // Create the Bot Framework Adapter.
            services.AddSingleton<BotFrameworkHttpAdapter, LivePersonAdapter>();

            services.AddSingleton<LivePersonConnector.ConversationMap>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, Bots.LivePersonProxyBot>();

            services.AddTransient<LivePersonConnector.ILivePersonCredentialsProvider, LivePersonCredentialsProvider>();

            services.AddSingleton<LivePersonConnector.HandoffMiddleware>();
            // Add transcription logging so agent gets conversation history.  If
            // you don't want agent to see conversation history, comment out or
            // delete this next line of code.
            services.AddSingleton<LoggingMiddleware>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();

            app.UseMvc();
        }
    }
}
