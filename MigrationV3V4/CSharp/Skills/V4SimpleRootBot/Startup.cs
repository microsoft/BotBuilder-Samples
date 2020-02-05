// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Integration.AspNet.Core.Skills;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.BotBuilderSamples.SimpleRootBot.Authentication;
using Microsoft.BotBuilderSamples.SimpleRootBot.Bots;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples.SimpleRootBot
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // MvcOptions.EnableEndpointRouting = false;
            // .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true)

            //services
            //  .AddMvc()
            //  .SetCompatibilityVersion(CompatibilityVersion.Latest)
            //  .AddNewtonsoftJson();

            /*
            services.AddMvcCore(option => option.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(o => 
                { 
                    o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    o.SerializerSettings.Formatting = Formatting.Indented;
                    o.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    o.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                    o.SerializerSettings.ContractResolver = new ReadOnlyJsonContractResolver();
                    o.SerializerSettings.Converters = new List<JsonConverter> { new Iso8601TimeSpanConverter() };
                 });

            //      services.TryAddSingleton<IActionResultExecutor<JsonResult>, SystemTextJsonResultExecutor>();

            // NewtonsoftJsonMvcCoreBuilderExtensions.AddNewtonsoftJson();
            */

            //services.Add(new ServiceDescriptor(
            //    typeof(IActionResultExecutor<JsonResult>),
            //    Type.GetType("Microsoft.AspNetCore.Mvc.Infrastructure.SystemTextJsonResultExecutor, Microsoft.AspNetCore.Mvc.Core"),
            //    ServiceLifetime.Singleton));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

//#if NETCOREAPP3_0
//            result.SerializerSettings = HttpHelper.BotMessageSerializerSettings as object;
//#endif

            // Configure credentials
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            // Register the skills configuration class
            services.AddSingleton<SkillsConfiguration>();

            // Register AuthConfiguration to enable custom claim validation.
            services.AddSingleton(sp => new AuthenticationConfiguration { ClaimsValidator = new AllowedSkillsClaimsValidator(sp.GetService<SkillsConfiguration>()) });

            // Register the Bot Framework Adapter with error handling enabled.
            // Note: some classes use the base BotAdapter so we add an extra registration that pulls the same instance.
            services.AddSingleton<BotFrameworkHttpAdapter, AdapterWithErrorHandler>();
            services.AddSingleton<BotAdapter>(sp => sp.GetService<BotFrameworkHttpAdapter>());

            // Register the skills client and skills request handler.
            services.AddSingleton<SkillConversationIdFactoryBase, SkillConversationIdFactory>();
            services.AddHttpClient<SkillHttpClient>();
            services.AddSingleton<ChannelServiceHandler, SkillHandler>();

            // Register the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            // Register Conversation state (used by the Dialog system itself).
            services.AddSingleton<ConversationState>();

            // Register the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, RootBot>();
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

            // app.UseHttpsRedirection();
            app.UseMvc();

            //app.UseRouting();

            //app.UseEndpoints((endpoints) => endpoints.MapDefaultControllerRoute());
        }
    }
}
