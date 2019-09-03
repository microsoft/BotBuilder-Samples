// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Configuration;
using System.Web.Http;
using Microsoft.Bot.Builder.Integration.AspNet.WebApi.Handlers;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;

namespace Microsoft.Bot.Builder.Integration.AspNet.WebApi
{
    public static class HttpConfigurationExtensions
    {
        /// <summary>
        /// Map the Bot Framework into the request execution pipeline.
        /// </summary>
        /// <param name="httpConfiguration">The <see cref="HttpConfiguration" /> to map the bot into.</param>
        /// <param name="configurer">A callback to configure the bot.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static HttpConfiguration MapBotFramework(this HttpConfiguration httpConfiguration, Action<BotFrameworkConfigurationBuilder> configurer = null)
        {
            if (httpConfiguration == null)
            {
                throw new ArgumentNullException(nameof(httpConfiguration));
            }

            var options = new BotFrameworkOptions();
            var optionsBuilder = new BotFrameworkConfigurationBuilder(options);

            configurer?.Invoke(optionsBuilder);

            var adapter = httpConfiguration.DependencyResolver.GetService(typeof(IAdapterIntegration)) as IAdapterIntegration;
            if (adapter == null)
            {
                var credentialProvider = ResolveCredentialProvider(options);

                var botFrameworkAdapter = new BotFrameworkAdapter(credentialProvider, options.AuthenticationConfiguration, options.ChannelProvider, options.ConnectorClientRetryPolicy, options.HttpClient);

                // error handler
                botFrameworkAdapter.OnTurnError = options.OnTurnError;

                // add middleware
                foreach (var middleware in options.Middleware)
                {
                    botFrameworkAdapter.Use(middleware);
                }

                adapter = botFrameworkAdapter;
            }

            ConfigureBotRoutes(httpConfiguration, options, adapter);

            ConfigureCustomEndpoints();

            return httpConfiguration;
        }

        private static void ConfigureBotRoutes(HttpConfiguration httpConfiguration, BotFrameworkOptions options, IAdapterIntegration adapter)
        {
            var routes = httpConfiguration.Routes;
            var baseUrl = options.Paths.BasePath;

            routes.MapHttpRoute(
                    BotMessageHandler.RouteName,
                    baseUrl.Trim('/') + "/" + options.Paths.MessagesPath.Trim('/'),
                    defaults: null,
                    constraints: null,
                    handler: new BotMessageHandler(adapter));
        }

        private static ICredentialProvider ResolveCredentialProvider(BotFrameworkOptions options)
        {
            var credentialProvider = options.CredentialProvider;

            // If a credential provider was explicitly configured, just return that straight away
            if (credentialProvider != null)
            {
                return credentialProvider;
            }

            return new SimpleCredentialProvider(ConfigurationManager.AppSettings[MicrosoftAppCredentials.MicrosoftAppIdKey], ConfigurationManager.AppSettings[MicrosoftAppCredentials.MicrosoftAppPasswordKey]);
        }

        /// <summary>
        /// Sets custom endpoints for the Open ID Metadata Document and OAuth API Endpoint from the App Settings.
        /// </summary>
        private static void ConfigureCustomEndpoints()
        {
            var openIdEndpoint = ConfigurationManager.AppSettings[AuthenticationConstants.BotOpenIdMetadataKey];

            if (!string.IsNullOrEmpty(openIdEndpoint))
            {
                ChannelValidation.OpenIdMetadataUrl = openIdEndpoint;
                GovernmentChannelValidation.OpenIdMetadataUrl = openIdEndpoint;
            }

            var oauthApiEndpoint = ConfigurationManager.AppSettings[AuthenticationConstants.OAuthUrlKey];

            if (!string.IsNullOrEmpty(oauthApiEndpoint))
            {
                OAuthClientConfig.OAuthEndpoint = oauthApiEndpoint;
            }

            var emulateOAuthCards = ConfigurationManager.AppSettings[AuthenticationConstants.EmulateOAuthCardsKey];

            if (!string.IsNullOrEmpty(emulateOAuthCards) && bool.TryParse(emulateOAuthCards, out bool emualteOAuthCardsValue))
            {
                OAuthClientConfig.EmulateOAuthCards = emualteOAuthCardsValue;
            }
        }
    }
}
