// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.Skills;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.BotBuilderSamples.DialogRootBot.Bots;
using Microsoft.BotBuilderSamples.DialogRootBot.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.BotBuilderSamples.DialogRootBot
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
            services.AddControllers()
                .AddNewtonsoftJson();

            // Register the skills configuration class.
            services.AddSingleton<SkillsConfiguration>();

            // Register AuthConfiguration to enable custom claim validation.
            services.AddSingleton(sp =>
            {
                var allowedSkills = sp.GetService<SkillsConfiguration>().Skills.Values.Select(s => s.AppId).ToList();

                var claimsValidator = new AllowedSkillsClaimsValidator(allowedSkills);

                // If TenantId is specified in config, add the tenant as a valid JWT token issuer for Bot to Skill conversation.
                // The token issuer for MSI and single tenant scenarios will be the tenant where the bot is registered.
                var validTokenIssuers = new List<string>();
                var tenantId = sp.GetService<IConfiguration>().GetSection(MicrosoftAppCredentials.MicrosoftAppTenantIdKey)?.Value;

                if (!string.IsNullOrWhiteSpace(tenantId))
                {
                    // For SingleTenant/MSI auth, the JWT tokens will be issued from the bot's home tenant.
                    // Therefore, these issuers need to be added to the list of valid token issuers for authenticating activity requests.
                    validTokenIssuers.Add(string.Format(CultureInfo.InvariantCulture, AuthenticationConstants.ValidTokenIssuerUrlTemplateV1, tenantId));
                    validTokenIssuers.Add(string.Format(CultureInfo.InvariantCulture, AuthenticationConstants.ValidTokenIssuerUrlTemplateV2, tenantId));
                    validTokenIssuers.Add(string.Format(CultureInfo.InvariantCulture, AuthenticationConstants.ValidGovernmentTokenIssuerUrlTemplateV1, tenantId));
                    validTokenIssuers.Add(string.Format(CultureInfo.InvariantCulture, AuthenticationConstants.ValidGovernmentTokenIssuerUrlTemplateV2, tenantId));
                }

                return new AuthenticationConfiguration
                {
                    ClaimsValidator = claimsValidator,
                    ValidTokenIssuers = validTokenIssuers
                };
            });

            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

            // Register the Bot Framework Adapter with error handling enabled.
            // Note: some classes expect a BotAdapter and some expect a BotFrameworkHttpAdapter, so
            // register the same adapter instance for both types.
            services.AddSingleton<CloudAdapter, AdapterWithErrorHandler>();
            services.AddSingleton<IBotFrameworkHttpAdapter>(sp => sp.GetService<CloudAdapter>());
            services.AddSingleton<BotAdapter>(sp => sp.GetService<CloudAdapter>());

            // Register the skills conversation ID factory, the client and the request handler.
            services.AddSingleton<SkillConversationIdFactoryBase, SkillConversationIdFactory>();
            services.AddSingleton(sp => sp.GetService<BotFrameworkAuthentication>().CreateBotFrameworkClient());
            services.AddSingleton<CloudChannelServiceHandler, CloudSkillHandler>();

            // Register the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            // Register Conversation state (used by the Dialog system itself).
            services.AddSingleton<ConversationState>();

            // Register the MainDialog that will be run by the bot.
            services.AddSingleton<MainDialog>();

            // Register the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, RootBot<MainDialog>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Uncomment this to support HTTPS.
            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseWebSockets();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
