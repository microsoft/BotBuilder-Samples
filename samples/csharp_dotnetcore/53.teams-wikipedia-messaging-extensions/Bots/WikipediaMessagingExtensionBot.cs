// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams.WikipediaMessagingExtension.Engine;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class WikipediaMessagingExtensionBot : TeamsActivityHandler
    {
        private readonly ISearchHandler _searchHandler;

        public WikipediaMessagingExtensionBot(ISearchHandler searchHandler)
        {
            _searchHandler = searchHandler;
        }

        protected override async Task<InvokeResponse> OnMessagingExtensionQueryAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
        {
            try
            {
                return new InvokeResponse
                {
                    Body = new MessagingExtensionResponse
                    {
                        ComposeExtension = await _searchHandler.GetSearchResultAsync(query),
                    },
                    Status = 200,
                };
            }
            catch (Exception ex)
            {
                return new InvokeResponse
                {
                    Body = new MessagingExtensionResponse
                    {
                        ComposeExtension = new MessagingExtensionResult
                        {
                            Text = "Failed to search " + ex.Message,
                            Type = "message",
                        },
                    },
                    Status = 200,
                };
            }
        }
    }
}
