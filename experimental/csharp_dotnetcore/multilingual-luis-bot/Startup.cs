// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.Ai.LUIS;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.Translation;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Microsoft.Bot.Samples.Ai.Luis.Translator
{
    public class Startup
    {
        private readonly bool _isProduction = false;
        private ILoggerFactory _loggerFactory;

        public Startup(IHostingEnvironment env)
        {
            _isProduction = env.IsProduction();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var secretKey = Configuration.GetSection("botFileSecret")?.Value;
            var botFilePath = Configuration.GetSection("botFilePath")?.Value;

            // Loads .bot configuration file and adds a singleton that your Bot can access through dependency injection.
            BotConfiguration botConfig = null;
            try
            {
                botConfig = BotConfiguration.Load(botFilePath ?? @".\BotConfiguration.bot", secretKey);
            }
            catch
            {
                var msg = @"Error reading bot file. Please ensure you have valid botFilePath and botFileSecret set for your environment.
                            - You can find the botFilePath and botFileSecret in the Azure App Service application settings.
                            - If you are running this bot locally, consider adding a appsettings.json file with botFilePath and botFileSecret.
                            - See https://aka.ms/about-bot-file to learn more about .bot file its use and bot configuration.
                            ";
                throw new InvalidOperationException(msg);
            }

            services.AddSingleton(sp => botConfig ?? throw new InvalidOperationException($"The .bot config file could not be loaded. ({botConfig})"));

            // Retrieve current endpoint.
            var environment = _isProduction ? "production" : "development";
            var service = botConfig.Services.Where(s => s.Type == "endpoint" && s.Name == environment).FirstOrDefault();
            if (!(service is EndpointService endpointService))
            {
                throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{environment}'.");
            }

            // Translation key from settings
            var translatorKey = Configuration.GetValue<string>("translatorKey");

            if (string.IsNullOrEmpty(translatorKey))
            {
                throw new InvalidOperationException("Microsoft Text Translation API key is missing. Please add your translation key to the 'translatorKey' setting.");
            }

            services.AddBot<LuisTranslatorBot>(options =>
            {
                options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

                // Catches any errors that occur during a conversation turn and logs them to currently
                // configured ILogger.
                ILogger logger = _loggerFactory.CreateLogger<LuisTranslatorBot>();
                options.OnTurnError = async (context, exception) =>
                {
                    logger.LogError($"Exception caught : {exception}");
                    await context.SendActivityAsync("Sorry, it looks like something went wrong.");
                };

                // Configure bot connected services
                ConfigureBot(botConfig, out var luisRecognizer);

                var middleware = options.Middleware;
                middleware.Add(new TranslationMiddleware(new string[] { "en" }, translatorKey, ConfigurePatterns(), ConfigureCusomDictionary()));
                middleware.Add(new LuisRecognizerMiddleware(luisRecognizer));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseBotFramework();
        }

        private void ConfigureBot(BotConfiguration botConfiguration, out LuisRecognizer luisRecognizer)
        {
            luisRecognizer = null;
            foreach (var service in botConfiguration.Services)
            {
                switch (service.Type)
                {
                    case ServiceTypes.Luis:
                        {
                            var luis = (LuisService)service;
                            if (luis == null)
                            {
                                throw new InvalidOperationException("The LUIS service is not configured correctly in your '.bot' file.");
                            }

                            var app = new LuisApplication(luis.AppId, luis.AuthoringKey, luis.GetEndpoint());
                            luisRecognizer = new LuisRecognizer(app);
                            break;
                        }
                }
            }
        }

        private Dictionary<string, List<string>> ConfigurePatterns()
        {
            var path = Configuration.GetValue<string>("patternsPath");
            var json = File.ReadAllText(path);
            var patterns = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);
            return patterns;
        }

        private ConfiguredLanguageDictionary ConfigureCusomDictionary()
        {
            var path = Configuration.GetValue<string>("dictionaryPath");
            var json = File.ReadAllText(path);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
            var userConfiguredLanguageDictionary = new ConfiguredLanguageDictionary();
            foreach (KeyValuePair<string, Dictionary<string, string>> lang in dictionary)
            {
                userConfiguredLanguageDictionary.AddNewLanguageDictionary(lang.Key, lang.Value);
            }

            return userConfiguredLanguageDictionary;
        }
    }
}
