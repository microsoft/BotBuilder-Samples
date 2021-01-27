// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.SkillBot.Bots
{
    public class ChildBot<T> : ActivityHandler
        where T : Dialog
    {
        private readonly ConversationState _conversationState;
        private readonly Dialog _dialog;

        public ChildBot(Dialog dialog, ConversationState conversationState)
        {
            _dialog = dialog;
            _conversationState = conversationState;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            await _dialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);

            // Save any state changes that might have occurred during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnEventAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Name == "Sso")
            {
                await _dialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
            }
            else
            {
                // We didn't get an event name we can handle.
                await turnContext.SendActivityAsync(MessageFactory.Text($"Unrecognized EventName: \"{turnContext.Activity.Name}\".", inputHint: InputHints.IgnoringInput), cancellationToken);
            }
        }
    }
}
