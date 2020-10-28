// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    // This bot is derived (view DialogBot<T>) from the TeamsActivityHandler class currently included as part of this sample.
    public class TeamsBot<T> : DialogBot<T> 
        where T : Dialog
    {
        TokenExchangeHelper _tokenExchangeHelper;

        public TeamsBot(TokenExchangeHelper tokenExchangeHelper, ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
            : base(conversationState, userState, dialog, logger)
        {
            _tokenExchangeHelper = tokenExchangeHelper;
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Welcome to AuthenticationBot. Type anything to get logged in. Type 'logout' to sign-out."), cancellationToken);
                }
            }
        }

        protected override async Task OnTokenResponseEventAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            await _dialogManager.OnTurnAsync(turnContext, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnSignInInvokeAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            
            if (turnContext.Activity.Name == SignInConstants.TokenExchangeOperationName)
            {
                // The Token Exchange Helper will attempt the exchange, and if successful, it will cache the result
                // in TurnState.  This is then read by TokenExchangeOAuthPrompt, and processed accordingly.
                if (! await _tokenExchangeHelper.ShouldProcessTokenExchange(turnContext, cancellationToken))
                {
                    // If the token is not exchangeable, do not process this activity further.
                    // (The Token Exchange Helper will send the appropriate response if the token is not exchangeable)
                    return;
                }
            }

            await _dialogManager.OnTurnAsync(turnContext, cancellationToken).ConfigureAwait(false);
        }

        protected override async Task OnTeamsSigninVerifyStateAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with signin/verifystate from an Invoke Activity.");

            // The OAuth Prompt needs to see the Invoke Activity in order to complete the login process.

            // Run the Dialog with the new Invoke Activity.
            await _dialogManager.OnTurnAsync(turnContext, cancellationToken).ConfigureAwait(false);
        }
    }
}
