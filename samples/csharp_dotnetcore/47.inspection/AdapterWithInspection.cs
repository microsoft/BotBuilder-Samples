// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class AdapterWithInspection : BotFrameworkHttpAdapter
    {
        public AdapterWithInspection(IConfiguration configuration, InspectionState inspectionState, UserState userState, ConversationState conversationState)
            : base(configuration)
        {
            // Inspection needs credentiaols because it will be sending the Activities and User and Conversation State to the emulator
            var credentials = new MicrosoftAppCredentials(configuration["MicrosoftAppId"], configuration["MicrosoftAppPassword"]);

            Use(new InspectionMiddleware(inspectionState, userState, conversationState, credentials));
        }
    }
}
