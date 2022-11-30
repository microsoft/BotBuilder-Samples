// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace Microsoft.BotBuilderSamples
{
    // This class is a wrapper for the Microsoft Graph API
    // See: https://developer.microsoft.com/en-us/graph
    public class SimpleGraphClient
    {
        private readonly string _token;

        public SimpleGraphClient(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            _token = token;
        }

        // Searches the user's mail Inbox using the Microsoft Graph API
        public async Task<Message[]> SearchMailInboxAsync(string search)
        {
            var graphClient = GetAuthenticatedClient();
            var searchQuery = new QueryOption("search", search);
            var messages = await graphClient.Me.MailFolders.Inbox.Messages.Request(new List<Option>() { searchQuery }).GetAsync();
            return messages.Take(10).ToArray();
        }

        //Fetching user's profile 
        public async Task<User> GetMyProfile()
        {
            var graphClient = GetAuthenticatedClient();
            return await graphClient.Me.Request().GetAsync();
        }

        // Get an Authenticated Microsoft Graph client using the token issued to the user.
        private GraphServiceClient GetAuthenticatedClient()
        {
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    requestMessage =>
                    {
                        // Append the access token to the request.
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", _token);

                        // Get event times in the current time zone.
                        requestMessage.Headers.Add("Prefer", "outlook.timezone=\"" + TimeZoneInfo.Local.Id + "\"");

                        return Task.CompletedTask;
                    }));
            return graphClient;
        }
    }
}
