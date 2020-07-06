// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.EchoSkillBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Text.Contains("end") || turnContext.Activity.Text.Contains("stop"))
            {
                // Send End of conversation at the end.
                var messageText = $"ending conversation from the skill...";
                await turnContext.SendActivityAsync(MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput), cancellationToken);
                var endOfConversation = Activity.CreateEndOfConversationActivity();
                endOfConversation.Code = EndOfConversationCodes.CompletedSuccessfully;
                await turnContext.SendActivityAsync(endOfConversation, cancellationToken);
            }
            else
            {
                var messageText = $"Echo: {turnContext.Activity.Text}";
                await turnContext.SendActivityAsync(MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput), cancellationToken);
                messageText = "Say \"end\" or \"stop\" and I'll end the conversation and back to the parent.";
                await turnContext.SendActivityAsync(MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput), cancellationToken);
            }
        }

        protected override Task OnEndOfConversationActivityAsync(ITurnContext<IEndOfConversationActivity> turnContext, CancellationToken cancellationToken)
        {
            // This will be called if the root bot is ending the conversation.  Sending additional messages should be
            // avoided as the conversation may have been deleted.
            // Perform cleanup of resources if needed.
            return Task.CompletedTask;
        }
    }
}
