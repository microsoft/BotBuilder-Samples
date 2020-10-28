// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Provides the ability to exchange a token for SSO, without the OAuthPrompt.
    /// A token exchange is attempted, and the result cached if successful.
    /// This is expected to be used in conjunction with TokenExchangeOauthPrompt,
    /// which will check for the cached token and respond appropriately.
    /// </summary>
    public class TokenExchangeHelper
    {
        private readonly IStorage _storage;
        private readonly string _oAuthConnectionName;


        public TokenExchangeHelper(IConfiguration configuration, IStorage storage)
        {
            _oAuthConnectionName = configuration["ConnectionName"];
            _storage = storage;
        }

        /// <summary>
        /// Determines if a "signin/tokenExchange" should be processed by this caller.
        ///
        /// If a token exchange is unsuccessful, an InvokeResponse of PreconditionFailed is sent.
        /// </summary>
        /// <param name="turnContext"><see cref="ITurnContext"/> for this specific activity.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> for this specific process.</param>
        /// <returns>True if the bot should continue processing this TokenExchange request.</returns>
        public async Task<bool> ShouldProcessTokenExchange(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Name == SignInConstants.TokenExchangeOperationName)
            {
                throw new InvalidOperationException("Only 'signin/tokenExchange' invoke activities can be procssed by TokenExchangeHelper.");
            }

            if (!await this.ExchangedTokenAsync(turnContext, cancellationToken).ConfigureAwait(false))
            {
                // If the TokenExchange is NOT successful, the response will have already been sent by ExchangedTokenAsync
                return false;
            }

            // If a user is signed into multiple Teams clients, the Bot might receive a "signin/tokenExchange" from each client.
            // Each token exchange request for a specific user login will have an identical Activity.Value.Id.
            // Only one of these token exchange requests should be processe by the bot.  For a distributed bot in production,
            // this requires a distributed storage to ensure only one token exchange is processed.

            // This example utilizes Bot Framework IStorage's ETag implementation for token exchange activity deduplication.

            // Create a StoreItem with Etag of the unique 'signin/tokenExchange' request
            var storeItem = new TokenStoreItem
            {
                ETag = (turnContext.Activity.Value as JObject).Value<string>("id")
            };

            var storeItems = new Dictionary<string, object> { { TokenStoreItem.GetStorageKey(turnContext), storeItem } };
            try
            {
                // Writing the IStoreItem with ETag of unique id will succeed only once
                await _storage.WriteAsync(storeItems, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
                // Memory storage throws a generic exception with a Message of 'Etag conflict. [other error info]'
                when (ex.Message.StartsWith("Etag conflict"))
            {
                // CosmosDbPartitionedStorage throws: ex.Message.Contains("pre-condition is not met")


                // Do NOT send this message on to be processed, some other thread or machine already has processed it.

                // TODO: Should send 200 invoke response here???
                return false;
            }

            return true;
        }

        private async Task<bool> ExchangedTokenAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            TokenResponse tokenExchangeResponse = null;
            var tokenExchangeRequest = ((JObject)turnContext.Activity.Value)?.ToObject<TokenExchangeInvokeRequest>();

            try
            {
                tokenExchangeResponse = await (turnContext.Adapter as IExtendedUserTokenProvider).ExchangeTokenAsync(
                    turnContext,
                    _oAuthConnectionName,
                    turnContext.Activity.From.Id,
                    new TokenExchangeRequest { Token = tokenExchangeRequest.Token },
                    cancellationToken).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types (ignoring, see comment below)
            catch
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // Ignore Exceptions
                // If token exchange failed for any reason, tokenExchangeResponse above stays null , and hence we send back a failure invoke response to the caller.
            }

            if (tokenExchangeResponse == null || string.IsNullOrEmpty(tokenExchangeResponse.Token))
            {
                // The token could not be exchanged (which could be due to a consent requirement)
                // Notify the sender that PreconditionFailed so they can respond accordingly.
                await turnContext.SendActivityAsync(
                    new Activity
                    {
                        Type = ActivityTypesEx.InvokeResponse,
                        Value = new InvokeResponse
                        {
                            Status = (int)HttpStatusCode.PreconditionFailed,
                            Body = new TokenExchangeInvokeResponse
                            {
                                Id = tokenExchangeRequest.Id,
                                ConnectionName = _oAuthConnectionName,
                                FailureDetail = "The bot is unable to exchange token. Proceed with regular login.",
                            },
                        },
                    }, cancellationToken).ConfigureAwait(false);

                return false;
            }
            else
            {
                // Store response in TurnState, so the TokenExchangeOAuthPrompt can use it, and not have to do the exchange again.
                turnContext.TurnState[nameof(TokenExchangeInvokeRequest)] = tokenExchangeRequest;
                turnContext.TurnState[nameof(TokenResponse)] = tokenExchangeResponse;
            }

            return true;
        }

        private class TokenStoreItem : IStoreItem
        {
            public string ETag { get; set; }

            public static string GetStorageKey(ITurnContext turnContext)
            {
                var activity = turnContext.Activity;
                var channelId = activity.ChannelId ?? throw new InvalidOperationException("invalid activity-missing channelId");
                var conversationId = activity.Conversation?.Id ?? throw new InvalidOperationException("invalid activity-missing Conversation.Id");

                if (activity.Type != ActivityTypes.Invoke || activity.Name != SignInConstants.TokenExchangeOperationName)
                {
                    throw new InvalidOperationException("TokenExchangeState can only be used with Invokes of signin/tokenExchange.");
                }

                var value = activity.Value as JObject;
                if (value == null || !value.ContainsKey("id"))
                {
                    throw new InvalidOperationException("Invalid signin/tokenExchange. Missing activity.Value.Id.");
                }

                return $"{channelId}/{conversationId}/{value.Value<string>("id")}";
            }
        }
    }
}
