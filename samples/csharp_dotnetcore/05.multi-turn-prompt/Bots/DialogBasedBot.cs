// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class DialogBasedBot : ActivityHandler
    {
        private ConversationState _conversationState;
        private UserState _userState;
        private UserProfileDialog _dialog;
        private ILogger<DialogBasedBot> _logger;

        public DialogBasedBot(ConversationState conversationState, UserState userState, UserProfileDialog dialog, ILogger<DialogBasedBot> logger)
        {
            _conversationState = conversationState;
            _userState = userState;
            _dialog = dialog;
            _logger = logger;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with Message Activity.");

            // Run the Dialog with the new message Activity.
            await _dialog.Run(turnContext, _conversationState, cancellationToken);

            // Save any state changes. The load happened during the execution of the Dialog. 
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
    }
}
