// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    public class LogoutDialog : ComponentDialog
    {
        protected ConversationState _conversationState;
        public const string CardChoicePropertyName = "CardChoice";

        public LogoutDialog(string id, string connectionName, ConversationState conversationState)
            : base(id)
        {
            _conversationState = conversationState;
            ConnectionName = connectionName;
        }

        protected string ConnectionName { get; }

        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnBeginDialogAsync(innerDc, options, cancellationToken);
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }
            var userChoice = await _conversationState.CreateProperty<string>(CardChoicePropertyName).GetAsync(innerDc.Context,()=> string.Empty, cancellationToken);

            if (userChoice == MainDialog.SignIn_Card && IsTeamsVerificationInvoke(innerDc.Context))
            {
                var magicCodeObject = innerDc.Context.Activity.Value as JObject;
                var magicCode = magicCodeObject.GetValue("state")?.ToString();

                var token = await (innerDc.Context.Adapter as IUserTokenProvider).GetUserTokenAsync(innerDc.Context, ConnectionName, magicCode, cancellationToken).ConfigureAwait(false);

                if (token != null)
                {
                    // Hack to support SignIn Cards in this LogoutDialog
                    innerDc.Context.Activity.Type = ActivityTypes.Message;
                    innerDc.Context.Activity.Text = token.ConnectionName;                    
                }
                else
                {
                    await innerDc.Context.SendActivityAsync(new Activity { Type = ActivityTypesEx.InvokeResponse, Value = new InvokeResponse { Status = 404 } }, cancellationToken).ConfigureAwait(false);
                }
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        private bool IsTeamsVerificationInvoke(ITurnContext turnContext)
        {
            var activity = turnContext.Activity;
            return activity.Type == ActivityTypes.Invoke && activity.Name == "signin/verifyState";
        }

        private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (innerDc.Context.Activity.Type == ActivityTypes.Message)
            {
                var cachedText = innerDc.Context.Activity.Text;
                // Hack to remove \n, which is currently appended to the end of a message when the bot is @mentioned
                var text = innerDc.Context.Activity.RemoveRecipientMention().ToLowerInvariant().Replace("\n", string.Empty).Trim();
                innerDc.Context.Activity.Text = cachedText;

                if (text.Equals("logout", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    // The bot adapter encapsulates the authentication processes.
                    var botAdapter = (BotFrameworkAdapter)innerDc.Context.Adapter;
                    await botAdapter.SignOutUserAsync(innerDc.Context, ConnectionName, null, cancellationToken);
                    await innerDc.Context.SendActivityAsync(MessageFactory.Text("You have been signed out."), cancellationToken);
                    return await innerDc.CancelAllDialogsAsync(cancellationToken);
                }
            }

            return null;
        }
    }
}
