// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Microsoft.Bot.Sample.PizzaBot
{
    internal static class ConversationHelper
    {
        private static readonly ConcurrentDictionary<string, ConnectorClient> _connectorClientCache = new ConcurrentDictionary<string, ConnectorClient>();

        /// <summary>
        /// Helper method that sends an `endOfConversation` activity.
        /// </summary>
        /// <param name="incomingActivity">The incoming user activity for this turn.</param>
        /// <param name="order">Optional. The completed order.</param>
        /// <param name="endOfConversationCode">Optional. The EndOfConversationCode to send to the parent bot.
        /// Defaults to EndOfConversationCodes.CompletedSuccessfully.</param>
        /// <remarks>Sending the `endOfConversation` activity when the conversation completes allows
        /// the bot to be consumed as a skill.</remarks>
        internal static async Task EndConversation(Activity incomingActivity, PizzaOrder order = null, string endOfConversationCode = EndOfConversationCodes.CompletedSuccessfully)
        {
            var connectorClient = _connectorClientCache.GetOrAdd(incomingActivity.ServiceUrl, key =>
            {
                var appId = ConfigurationManager.AppSettings[MicrosoftAppCredentials.MicrosoftAppIdKey];
                var appPassword = ConfigurationManager.AppSettings[MicrosoftAppCredentials.MicrosoftAppPasswordKey];
                return new ConnectorClient(new Uri(incomingActivity.ServiceUrl), appId, appPassword);
            });

            // Send End of conversation as reply.
            await connectorClient.Conversations.SendToConversationAsync(incomingActivity.CreateReply("Ending conversation from the skill..."));
            var endOfConversation = incomingActivity.CreateReply();
            if (order != null)
            {
                endOfConversation.Value = JsonConvert.SerializeObject(order);
            }
            endOfConversation.Type = ActivityTypes.EndOfConversation;
            endOfConversation.Code = endOfConversationCode;
            await connectorClient.Conversations.SendToConversationAsync(endOfConversation);
        }

        /// <summary>
        /// Clear the dialog stack and data bags.
        /// </summary>
        /// <param name="activity">The incoming activity to use for scoping the Conversation.Container.</param>
        internal static async Task ClearState(Activity activity)
        {
            // This is required for PVA manifest validation.
            // PVA will send an EOC activity with null Recipient.
            if (activity.Recipient == null)
                return;

            using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
            {
                var botData = scope.Resolve<IBotData>();
                await botData.LoadAsync(default(CancellationToken));

                // Some skills might persist data between invokations.
                botData.UserData.Clear();
                botData.ConversationData.Clear();
                botData.PrivateConversationData.Clear();

                var stack = scope.Resolve<IDialogStack>();
                stack.Reset();
                
                await botData.FlushAsync(default(CancellationToken));
            }
        }
    }
}
