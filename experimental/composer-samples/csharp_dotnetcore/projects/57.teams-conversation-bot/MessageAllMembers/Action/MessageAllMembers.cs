// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;

namespace Microsoft.Composer.Samples.Component.MessageAllMembers
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageAllMembers : Dialog
    {
        [JsonConstructor]
        public MessageAllMembers([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            // enable instances of this command as debug break point
            this.RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        [JsonProperty("$kind")]
        public const string Kind = "MessageAllMembers";

        public override async Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await MessageAllMembersAsync(dc.Context, cancellationToken);

            return await dc.EndDialogAsync();
        }

        private async Task MessageAllMembersAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var teamsChannelId = turnContext.Activity.TeamsGetChannelId();
            var serviceUrl = turnContext.Activity.ServiceUrl;
            var credentials = turnContext.TurnState.Get<IConnectorClient>()?.Credentials as MicrosoftAppCredentials;
            ConversationReference conversationReference = null;

            if (credentials == null)
            {
                throw new InvalidOperationException($"Missing credentials as {nameof(MicrosoftAppCredentials)} in {nameof(IConnectorClient)} from TurnState");
            }

            var members = await GetPagedMembers(turnContext, cancellationToken);

            foreach (var teamMember in members)
            {
                var proactiveMessage = MessageFactory.Text($"Hello {teamMember.GivenName} {teamMember.Surname}. I'm a Teams conversation bot.");

                var conversationParameters = new ConversationParameters
                {
                    IsGroup = false,
                    Bot = turnContext.Activity.Recipient,
                    Members = new ChannelAccount[] { teamMember },
                    TenantId = turnContext.Activity.Conversation.TenantId,
                };

                await CreateConversationAsync(
                    turnContext,
                    teamsChannelId,
                    serviceUrl,
                    credentials,
                    conversationParameters,
                    async (t1, c1) =>
                    {
                        conversationReference = t1.Activity.GetConversationReference();
                        await ((CloudAdapterBase)turnContext.Adapter).ContinueConversationAsync(
                            credentials.MicrosoftAppId,
                            conversationReference,
                            async (t2, c2) =>
                            {
                                await t2.SendActivityAsync(proactiveMessage, c2);
                            },
                            cancellationToken);
                    },
                    cancellationToken);
            }

            await turnContext.SendActivityAsync(MessageFactory.Text("All messages have been sent."), cancellationToken);
        }

        private async Task CreateConversationAsync(ITurnContext turnContext, string channelId, string serviceUrl, AppCredentials credentials, ConversationParameters conversationParameters, BotCallbackHandler callback, CancellationToken cancellationToken)
        {
            using (var connectorClient = new ConnectorClient(new Uri(serviceUrl), credentials))
            {
                var result = await connectorClient.Conversations.CreateConversationAsync(conversationParameters, cancellationToken).ConfigureAwait(false);

                // Create a conversation update activity to represent the result.
                var eventActivity = Activity.CreateEventActivity();
                eventActivity.Name = ActivityEventNames.CreateConversation;
                eventActivity.ChannelId = channelId;
                eventActivity.ServiceUrl = serviceUrl;
                eventActivity.Id = result.ActivityId ?? Guid.NewGuid().ToString("n");
                eventActivity.Conversation = new ConversationAccount(id: result.Id, tenantId: conversationParameters.TenantId);
                eventActivity.ChannelData = conversationParameters.ChannelData;
                eventActivity.Recipient = conversationParameters.Bot;

                using (var context = new TurnContext(turnContext.Adapter, (Activity)eventActivity))
                {
                    var claimsIdentity = new ClaimsIdentity();
                    claimsIdentity.AddClaim(new Claim(AuthenticationConstants.AudienceClaim, credentials.MicrosoftAppId));
                    claimsIdentity.AddClaim(new Claim(AuthenticationConstants.AppIdClaim, credentials.MicrosoftAppId));
                    claimsIdentity.AddClaim(new Claim(AuthenticationConstants.ServiceUrlClaim, serviceUrl));

                    //context.TurnState.Add<IIdentity>(BotIdentityKey, claimsIdentity);
                    //context.TurnState.Add(connectorClient);

                    //Middleware?  Forget about it in this little hack.
                    //await RunPipelineAsync(context, callback, cancellationToken).ConfigureAwait(false);
                    await callback(turnContext, cancellationToken).ConfigureAwait(false);

                    // Cleanup disposable resources in case other code kept a reference to it.
                    //context.TurnState.Set<IConnectorClient>(null);
                }
            }
        }

        private static async Task<List<TeamsChannelAccount>> GetPagedMembers(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var members = new List<TeamsChannelAccount>();
            string continuationToken = null;

            do
            {
                var currentPage = await TeamsInfo.GetPagedMembersAsync(turnContext, 100, continuationToken, cancellationToken);
                continuationToken = currentPage.ContinuationToken;
                members = members.Concat(currentPage.Members).ToList();
            }
            while (continuationToken != null);

            return members;
        }
    }
}
