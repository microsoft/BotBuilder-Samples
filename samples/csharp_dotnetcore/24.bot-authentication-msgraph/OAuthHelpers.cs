// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Graph;

namespace Microsoft.BotBuilderSamples
{
    // This class calls the Microsoft Graph API. The following OAuth scopes are used:
    // 'openid' 'profile' 'User.Read'
    // for more information about scopes see:
    // https://developer.microsoft.com/en-us/graph/docs/concepts/permissions_reference
    public static class OAuthHelpers
    {
        // Send the user their Graph Display Name from the bot.
        public static async Task ListMeAsync(ITurnContext turnContext, TokenResponse tokenResponse)
        {
            var user = await GetUserAsync(turnContext, tokenResponse);
            await turnContext.SendActivityAsync($"You are {user.DisplayName}.");
        }

        // Send the user their Graph Email Address from the bot.
        public static async Task ListEmailAddressAsync(ITurnContext turnContext, TokenResponse tokenResponse)
        {
            var user = await GetUserAsync(turnContext, tokenResponse);
            await turnContext.SendActivityAsync($"Your email: {user.Mail}.");
        }

        private static async Task<User> GetUserAsync(ITurnContext turnContext, TokenResponse tokenResponse)
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (tokenResponse == null)
            {
                throw new ArgumentNullException(nameof(tokenResponse));
            }

            // Pull in the data from the Microsoft Graph.
            var client = new SimpleGraphClient(tokenResponse.Token);
            return await client.GetMeAsync();
        }
    }
}
