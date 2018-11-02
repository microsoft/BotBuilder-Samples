// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// A factory for the adapters we will be using, common code across each of the controllers.
    /// </summary>
    public static class AdapterFactory
    {
        public static IAdapterIntegration Create(BotConfiguration botConfig, string name)
        {
            var endpointName = $"{name} development";

            var service = botConfig.Services.Where(s => s.Type == "endpoint" && s.Name == endpointName).FirstOrDefault();
            if (!(service is EndpointService endpointService))
            {
                throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{name}'.");
            }

            var credentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

            var adapter = new BotFrameworkAdapter(credentialProvider);

            adapter.OnTurnError = async (context, exception) =>
            {
                await context.SendActivityAsync($"Sorry, it looks like something went wrong in {name}.");
            };
            return adapter;
        }
    }
}
