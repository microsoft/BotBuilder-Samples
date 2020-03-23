// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.EchoSkillBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private readonly Dialog _dialog;
        private readonly ConversationState _conversationState;
        private readonly UserState _userState;

        public EchoBot(Dialog dialog, ConversationState conversationState, UserState userState)
        {
            _dialog = dialog;
            _conversationState = conversationState;
            _userState = userState;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnSignInInvokeAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            await _conversationState.LoadAsync(turnContext, true, cancellationToken);
            await _userState.LoadAsync(turnContext, true, cancellationToken);
            await _dialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        protected override async Task OnTokenResponseEventAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            await _conversationState.LoadAsync(turnContext, true, cancellationToken);
            await _userState.LoadAsync(turnContext, true, cancellationToken);
            await _dialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Text.Contains("end") || turnContext.Activity.Text.Contains("stop"))
            {
                // Send End of conversation at the end.
                await turnContext.SendActivityAsync(MessageFactory.Text($"ending conversation from the skill..."), cancellationToken);
                var endOfConversation = Activity.CreateEndOfConversationActivity();
                endOfConversation.Code = EndOfConversationCodes.CompletedSuccessfully;
                await turnContext.SendActivityAsync(endOfConversation, cancellationToken);
            }
            else
            {
                //await turnContext.SendActivityAsync(MessageFactory.Text($"Echo (dotnet) : {turnContext.Activity.Text}"), cancellationToken);
                //await turnContext.SendActivityAsync(MessageFactory.Text("Say \"end\" or \"stop\" and I'll end the conversation and back to the parent."), cancellationToken);
                await _conversationState.LoadAsync(turnContext, true, cancellationToken);
                await _userState.LoadAsync(turnContext, true, cancellationToken);
                await _dialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);

            }
        }

        protected override async Task OnEndOfConversationActivityAsync(ITurnContext<IEndOfConversationActivity> turnContext, CancellationToken cancellationToken)
        {
            // This will be called if the root bot is ending the conversation.  Sending additional messages should be
            // avoided as the conversation may have been deleted.
            // Perform cleanup of resources if needed.
            await _conversationState.LoadAsync(turnContext, true, cancellationToken);
            await _userState.LoadAsync(turnContext, true, cancellationToken);
            var dialogState = _conversationState.CreateProperty<DialogState>(nameof(DialogState));
            await dialogState.DeleteAsync(turnContext, cancellationToken);
        }
    }
}
