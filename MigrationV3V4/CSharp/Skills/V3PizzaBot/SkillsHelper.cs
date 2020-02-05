// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace Microsoft.Bot.Sample.PizzaBot
{
    public static class SkillsHelper
    {
        public static async Task EndSkillConversation(Activity incomingActivity)
        {
            var appId = ConfigurationManager.AppSettings[MicrosoftAppCredentials.MicrosoftAppIdKey];
            var appPassword = ConfigurationManager.AppSettings[MicrosoftAppCredentials.MicrosoftAppPasswordKey];
            var connector = new ConnectorClient(new Uri(incomingActivity.ServiceUrl), appId, appPassword);

            // Send End of conversation as reply.
            await connector.Conversations.SendToConversationAsync(incomingActivity.CreateReply("Ending conversation from the skill..."));
            var endOfConversation = incomingActivity.CreateReply();
            endOfConversation.Type = ActivityTypes.EndOfConversation;
            endOfConversation.Code = EndOfConversationCodes.CompletedSuccessfully;
            await connector.Conversations.SendToConversationAsync(endOfConversation);
        }
    }
}
