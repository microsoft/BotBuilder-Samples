// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.3.0

using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Debugging;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using Microsoft.Bot.Builder.Dialogs.Declarative.Types;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.LanguageGeneration.Renderer;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Debug;

namespace AdaptiveBotSample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            HostingEnvironment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

        }

        public IHostingEnvironment HostingEnvironment { get; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Create the credential provider to be used with the Bot Framework Adapter.
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    TelemetryConfiguration.Active.DisableTelemetry = true;
            //}

            // hook up debugging support
            var sourceMap = new SourceMap();
            DebugAdapter debugAdapter = null;
            bool enableDebugger = true;
            if (enableDebugger)
            {
                // by setting the source registry all dialogs will register themselves to be debugged as execution flows
                DebugSupport.SourceRegistry = sourceMap;
                debugAdapter = new DebugAdapter(sourceMap, sourceMap, new DebugLogger(nameof(DebugAdapter)));
            }

            services.AddSingleton(DebugSupport.SourceRegistry);

            // set the configuration for types
            TypeFactory.Configuration = this.Configuration;

            // register adaptive library components
            TypeFactory.RegisterAdaptiveTypes();

            // register custom components
            TypeFactory.Register("Testbot.CalculateDogYears", typeof(CalculateDogYears));
            TypeFactory.Register("Testbot.JavascriptStep", typeof(JavascriptStep));
            TypeFactory.Register("Testbot.CSharpStep", typeof(CSharpStep));

            IStorage dataStore = new MemoryStorage();
            services.AddSingleton(dataStore);

            var userState = new UserState(dataStore);
            services.AddSingleton(userState);

            var conversationState = new ConversationState(dataStore);
            services.AddSingleton(conversationState);

            var resourceExplorer = ResourceExplorer.LoadProject(HostingEnvironment.ContentRootPath);
            services.AddSingleton(resourceExplorer);

            services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>((s) =>
            {
                // manage all bot resources
                var lg = new LGLanguageGenerator(resourceExplorer);

                var adapter = new BotFrameworkHttpAdapter();

                adapter
                    .Use(new RegisterClassMiddleware<IStorage>(dataStore))
                    .Use(new RegisterClassMiddleware<ResourceExplorer>(resourceExplorer))
                    .Use(new RegisterClassMiddleware<ILanguageGenerator>(lg))
                    .Use(new RegisterClassMiddleware<IMessageActivityGenerator>(new TextMessageActivityGenerator(lg)))
                    .Use(new AutoSaveStateMiddleware(userState, conversationState));

                if (debugAdapter != null)
                {
                    adapter.Use(debugAdapter);
                }

                adapter.OnTurnError = async (turnContext, exception) =>
                {
                    await conversationState.ClearStateAsync(turnContext);
                    await conversationState.SaveChangesAsync(turnContext);
                };
                return adapter;
            });

            services.AddSingleton<IBot, Bot>();
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

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
