// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bot_Authentication;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This bot will respond to the user's input with an <see cref="OAuthPrompt"/>.
    /// </summary>
    public class AuthenticationBot : IBot
    {
        private const string ConnectionName = "AADv2Connection";

        private const string HelpText = " This bot will introduce you to Authentication." +
                                        " Type anything to get logged in. Type 'logout' to signout." +
                                        " Type 'help' to view this message again";

        private readonly AuthenticationBotAccessors _stateAccessors;
        private readonly DialogSet _dialogs;

        public AuthenticationBot(AuthenticationBotAccessors accessors)
        {
            this._stateAccessors = accessors;
            this._dialogs = new DialogSet(this._stateAccessors.ConversationDialogState);
            this._dialogs.Add(Prompt(ConnectionName));
            this._dialogs.Add(new ConfirmPrompt("confirm"));
            this._dialogs.Add(new WaterfallDialog("authDialog", new WaterfallStep[] { PromptStepAsync, LoginStepAsync, DisplayTokenAsync }));
        }

        /// <summary>
        /// This controls what happens when an activity gets sent to the bot.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dc = await this._dialogs.CreateContextAsync(turnContext, cancellationToken);
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:

                    var text = turnContext.Activity.Text.ToLowerInvariant();
                    if (text == "help")
                    {
                        await turnContext.SendActivityAsync(HelpText, cancellationToken: cancellationToken);
                        break;
                    }

                    if (text == "logout")
                    {
                            // The bot adapter encapsulates authentication processes and sends
                            // activities to and receives activities from the Bot Connector Service.
                            var botAdapter = (BotFrameworkAdapter)turnContext.Adapter;
                            await botAdapter.SignOutUserAsync(turnContext, ConnectionName, cancellationToken);
                    }
                    await dc.ContinueAsync(cancellationToken);

                    if (!turnContext.Responded)
                    {
                        var result = await dc.BeginAsync("authDialog", cancellationToken: cancellationToken);
                        var token = (TokenResponse)result.Result;
                    }

                    break;
                case ActivityTypes.Event:
                case ActivityTypes.Invoke:
                    // This handles the MS Teams Invoke Activity sent when magic code is not used.
                    // See: https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/authentication/auth-oauth-card#getting-started-with-oauthcard-in-teams
                    // Manifest Schema Here: https://docs.microsoft.com/en-us/microsoftteams/platform/resources/schema/manifest-schema
                    // It also handles the Event Activity sent from The Emulator when the magic code is not used.
                    // See: https://blog.botframework.com/2018/08/28/testing-authentication-to-your-bot-using-the-bot-framework-emulator/
                    dc = await this._dialogs.CreateContextAsync(turnContext, cancellationToken);
                    await dc.ContinueAsync(cancellationToken);
                    if (!turnContext.Responded)
                    {
                        await dc.BeginAsync("authDialog", cancellationToken: cancellationToken);
                    }

                    break;
                case ActivityTypes.ConversationUpdate:
                    // Send a welcome & help message to the user.
                    if (turnContext.Activity.MembersAdded.Any())
                    {
                        await SendWelcomeMessageAsync(turnContext, cancellationToken);
                    }

                    break;
            }
        }

        /// <summary>
        /// Greet new users as they are added to the conversation.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome to AuthenticationBot {member.Name}.",
                        cancellationToken: cancellationToken);
                }
            }
        }

        // Prompts the user to log in using the OAuth provider specified by the connection name.
        private static OAuthPrompt Prompt(string connectionName)
        {
            return new OAuthPrompt(
                "loginPrompt",
                new OAuthPromptSettings
                {
                    ConnectionName = connectionName,
                    Text = "Please Sign In",
                    Title = "Sign In",
                    Timeout = 300000, // User has 5 minutes to login
                });
        }

        /// <summary>
        /// This <see cref="WaterfallStep"/> prompts the user to log in.
        /// </summary>
        /// <param name="dc">A <see cref="DialogContext"/> provides context for the current dialog.</param>
        /// <param name="step">A <see cref="WaterfallStepContext"/> provides context for the current waterfall step.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the operation.</returns>
        private static async Task<DialogTurnResult> PromptStepAsync(DialogContext dc, WaterfallStepContext step, CancellationToken cancellationToken)
        {
            return await dc.BeginAsync("loginPrompt", cancellationToken: cancellationToken);
        }

        /// <summary>
        /// In this step we check that a token was received and prompt the user asking if they would like
        /// to see the token or not.
        /// </summary>
        /// <param name="dc">A <see cref="DialogContext"/> provides context for the current dialog.</param>
        /// <param name="step">A <see cref="WaterfallStepContext"/> provides context for the current waterfall step.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the operation.</returns>
        private static async Task<DialogTurnResult> LoginStepAsync(DialogContext dc, WaterfallStepContext step, CancellationToken cancellationToken)
        {
            var tokenResponse = (TokenResponse)step.Result;
            if (tokenResponse != null)
            {
                await dc.Context.SendActivityAsync("You are now logged in.", cancellationToken: cancellationToken);
                return await dc.PromptAsync(
                    "confirm",
                    new PromptOptions
                    {
                        Prompt = MessageFactory.Text("Would you like to view your token?"),
                        Choices = new List<Choice> { new Choice("Yes"), new Choice("No") },
                    },
                    cancellationToken);
            }

            await dc.Context.SendActivityAsync("Login was not sucessful please try again", cancellationToken: cancellationToken);
            return Dialog.EndOfTurn;
        }

        /// <summary>
        /// Fetch the token and display it for the user if they asked to see it.
        /// </summary>
        /// <param name="dc">A <see cref="DialogContext"/> provides context for the current dialog.</param>
        /// <param name="step">A <see cref="WaterfallStepContext"/> provides context for the current waterfall step.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the operation.</returns>
        private static async Task<DialogTurnResult> DisplayTokenAsync(DialogContext dc, WaterfallStepContext step, CancellationToken cancellationToken)
        {
            var result = (bool)step.Result;
            if (result)
            {
                // Here we call the prompt again because we need the token. We do this for a couple of reasons.
                // If the user is already logged in we do not need to store the token locally in the bot and worry
                // about refreshing it. We can always just call the prompt again to get the token. Another reason we
                // do this is because in a bot we never know how long it will take a user to respond. By the time the
                // user responds the token may have expired. The user would then be prompted to login again. There is
                // no reason to store the token locally in the bot because we can always just call the OAuth prompt to
                // get the token or get a new token if needed.
                var prompt = await dc.BeginAsync("loginPrompt", cancellationToken: cancellationToken);
                var tokenResponse = (TokenResponse)prompt.Result;
                if (tokenResponse != null)
                {
                    await dc.Context.SendActivityAsync($"Here is your token {tokenResponse.Token}", cancellationToken: cancellationToken);
                }
            }

            return Dialog.EndOfTurn;
        }
    }
}
