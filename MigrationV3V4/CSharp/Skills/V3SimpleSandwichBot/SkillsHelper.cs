// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace Microsoft.Bot.Sample.SimpleSandwichBot
{
    internal static class SkillsHelper
    {
        /// <summary>
        /// Helper method that sends an `endOfConversation` activity.
        /// </summary>
        /// <param name="incomingActivity">The incoming user activity for this turn.</param>
        /// <param name="order">Optional. The completed order.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        /// <remarks>Sending the `endOfConversation` activity when the conversation completes allows
        /// the bot to be consumed as a skill.</remarks>
        internal static async Task EndSkillConversation(Activity incomingActivity, SandwichOrder order = null)
        {
            var appId = ConfigurationManager.AppSettings[MicrosoftAppCredentials.MicrosoftAppIdKey];
            var appPassword = ConfigurationManager.AppSettings[MicrosoftAppCredentials.MicrosoftAppPasswordKey];
            var connector = new ConnectorClient(new Uri(incomingActivity.ServiceUrl), appId, appPassword);

            // Send End of conversation as reply.
            await connector.Conversations.SendToConversationAsync(incomingActivity.CreateReply("Ending conversation from the skill..."));
            var endOfConversation = incomingActivity.CreateReply();
            if (order != null)
            {
                endOfConversation.Value = JsonConvert.SerializeObject(order);
            }
            endOfConversation.Type = ActivityTypes.EndOfConversation;
            endOfConversation.Code = EndOfConversationCodes.CompletedSuccessfully;
            await connector.Conversations.SendToConversationAsync(endOfConversation);
        }
    }
}
