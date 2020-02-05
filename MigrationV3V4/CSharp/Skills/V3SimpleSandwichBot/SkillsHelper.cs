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
