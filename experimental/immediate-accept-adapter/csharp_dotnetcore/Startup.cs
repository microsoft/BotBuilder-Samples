// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.9.2

using System;
using ImmediateAcceptBot.BackgroundQueue;
using ImmediateAcceptBot.Bots;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ImmediateAcceptBot
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
            services.AddControllers().AddNewtonsoftJson();

            // Activity specific BackgroundService for processing athenticated activities.
            services.AddHostedService<HostedActivityService>();
            // Generic BackgroundService for processing tasks.
            services.AddHostedService<HostedTaskService>();
            
            // BackgroundTaskQueue and ActivityTaskQueue are the entry points for
            // the enqueueing activities or tasks to be processed by the BackgroundService.
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddSingleton<IActivityTaskQueue, ActivityTaskQueue>();

            // Configure the ShutdownTimeout based on appsettings.
            services.Configure<HostOptions>(opts => opts.ShutdownTimeout = TimeSpan.FromSeconds(Configuration.GetValue<int>("ShutdownTimeoutSeconds")));

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<ImmediateAcceptAdapter>();

            // Create the bot. In this case the ASP Controller and ImmediateAcceptAdapter is expecting an IBot.
            services.AddSingleton<IBot, EchoBot>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }
    }
}
