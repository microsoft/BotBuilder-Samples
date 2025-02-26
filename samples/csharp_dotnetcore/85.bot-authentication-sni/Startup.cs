﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Security.Cryptography.X509Certificates;
using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Client;

namespace Microsoft.BotBuilderSamples
{
    public class Startup
    {
        public readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient().AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.MaxDepth = HttpHelper.BotMessageSerializerSettings.MaxDepth;
            });

            //
            // NOTE
            //
            // This is a provisional sample for SN+I.  It is likely to change in the future as this
            // functionality is rolled into BF SDK.
            //

            //Set sendX5C value to true to use SNI auhtentication.
            var sendX5C = true;

            // Using KeyVault.
            // Create a new certificate client using the default credential from Azure.Identity using environment variables previously set,
            // including AZURE_CLIENT_ID, AZURE_CLIENT_SECRET, and AZURE_TENANT_ID.
            var keyVaultUri = $"https://{_configuration["KeyVaultName"]}.vault.azure.net";
            var credential = new DefaultAzureCredential();
            var client = new CertificateClient(new Uri(keyVaultUri), credential);

            // Get certificate in X509Certificate format.
            var certificateName = _configuration["CertificateName"];
            var certificate = client.DownloadCertificate(certificateName).Value;

            // Using a local certificate.
            //var certificate = X509Certificate2.CreateFromPemFile(@"{Pem file path}");

            // Register CertificateServiceClientCredentialsFactory
            services.AddSingleton<ServiceClientCredentialsFactory>(
                new CertificateServiceClientCredentialsFactory(certificate, _configuration["MicrosoftAppId"], _configuration["MicrosoftAppTenantId"], null, null, sendX5C));

            // Create the Bot Framework Authentication to be used with the Bot Adapter.
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();

            // Create the Bot Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, AuthSNIBot>();
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
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
        }
    }
}
