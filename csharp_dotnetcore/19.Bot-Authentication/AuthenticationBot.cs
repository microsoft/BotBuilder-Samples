// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.using System;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace Bot_Authentication
{
    /// <summary>
    /// This bot uses OAuth to log the user in. The OAuth provider being demonstrated
    /// here is Azure Active Directory v2.0 (AADv2). The bot uses the Microsoft Graph
    /// API and the Outlook API to demonstrate making calls to a service that requires
    /// authentication. Bot developers no longer need to host OAuth controllers or
    /// manage the token life-cycle, as all of this can now be done by the Azure Bot Service.
    /// </summary>
    public class AuthenticationBot : IBot
    {
        // Your connection name
        private const string ConnectionSettingName = "";

        // Instructions for the user with information about commands that this bot may handle.
        private const string HelpText =
            "You can type 'send <recipient_email>' to send an email, 'recent' to view recent unread mail" +
            " 'me' to see information about yourself, or 'help' to view the commands" +
            " again. Any other text will display your token.";

        private readonly AuthenticationBotAccessors _stateAccessors;
        private readonly DialogSet _dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationBot"/> class.
        /// In the constructor for the bot we are instantiating our <see cref="DialogSet"/>, giving our field a value,
        /// adding our <see cref="WaterfallDialog"/> and <see cref="ChoicePrompt"/> to the dialog set.
        /// We are also adding  multiple <see cref="WaterfallStep"/> to our <see cref="WaterfallDialog"/>.
        /// </summary>
        /// <param name="accessors">State accessors for the bot.</param>
        public AuthenticationBot(AuthenticationBotAccessors accessors)
        {
            this._stateAccessors = accessors;
            this._dialogs = new DialogSet(this._stateAccessors.ConversationDialogState);
            this._dialogs.Add(OAuthHelpers.Prompt(ConnectionSettingName));
            this._dialogs.Add(new ChoicePrompt("choicePrompt"));
            this._dialogs.Add(new WaterfallDialog("graphDialog", new WaterfallStep[] { PromptStepAsync, ProcessStepAsync }));
        }

        /// <summary>
        /// This controls what happens when an <see cref="Activity"/> gets sent to the bot.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            DialogContext dc = null;

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:

                    dc = await ProcessInputAsync(turnContext, dc, cancellationToken);

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
                        await dc.BeginAsync("graphDialog", cancellationToken: cancellationToken);
                    }

                    break;
                case ActivityTypes.ConversationUpdate:
                    // Send a HeroCard as a welcome message when a new use joins the conversation
                    var newUserName = turnContext.Activity.MembersAdded.FirstOrDefault()?.Name;

                    if (!string.Equals("Bot", newUserName))
                    {
                        var reply = turnContext.Activity.CreateReply();
                        reply.Text = HelpText;
                        reply.Attachments = new List<Attachment> { GetHeroCard(newUserName).ToAttachment() };
                        await turnContext.SendActivityAsync(reply, cancellationToken);
                    }

                    break;
            }
        }

        /// <summary>
        /// Creates a <see cref="HeroCard"/> that is sent as a welcome message to the user.
        /// </summary>
        /// <param name="newUserName"> The name of the user.</param>
        /// <returns>A <see cref="HeroCard"/> the user can interact with.</returns>
        private static HeroCard GetHeroCard(string newUserName)
        {
            var heroCard = new HeroCard($"Welcome {newUserName}", "OAuthBot")
            {
                Images = new List<CardImage>
                {
                    new CardImage(
                        "https://jasonazurestorage.blob.core.windows.net/files/aadlogo.png",
                        "AAD Logo",
                        new CardAction(
                            ActionTypes.OpenUrl,
                            value: "https://ms.portal.azure.com/#blade/Microsoft_AAD_IAM/ActiveDirectoryMenuBlade/Overview")),
                },
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.ImBack, "Me", text: "Me", displayText: "Me", value: "Me"),
                    new CardAction(ActionTypes.ImBack, "Recent", text: "Recent", displayText: "Recent", value: "Recent"),
                    new CardAction(ActionTypes.ImBack, "View Token", text: "View Token", displayText: "View Token", value: "View Token"),
                    new CardAction(ActionTypes.ImBack, "Help", text: "Help", displayText: "Help", value: "Help"),
                    new CardAction(ActionTypes.ImBack, "Signout", text: "Signout", displayText: "Signout", value: "Signout"),
                },
            };
            return heroCard;
        }

        /// <summary>
        /// Processes the user's input and routes the user to the appropriate step.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="dc">A <see cref="DialogContext"/> provides context for the current dialog.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the operation.</returns>
        private async Task<DialogContext> ProcessInputAsync(ITurnContext turnContext, DialogContext dc, CancellationToken cancellationToken)
        {
            dc = await this._dialogs.CreateContextAsync(turnContext, cancellationToken);
            switch (turnContext.Activity.Text.ToLowerInvariant())
            {
                case "signout":
                case "logout":
                case "signoff":
                case "logoff":
                    // The bot adapter encapsulates authentication processes and sends
                    // activities to and receives activities from the Bot Connector Service.
                    var botAdapter = (BotFrameworkAdapter)turnContext.Adapter;
                    await botAdapter.SignOutUserAsync(turnContext, ConnectionSettingName, cancellationToken);

                    // Let the user know they are signed out/
                    await turnContext.SendActivityAsync("You are now signed out.", cancellationToken: cancellationToken);
                    break;
                case "help":
                    await turnContext.SendActivityAsync(HelpText, cancellationToken: cancellationToken);
                    break;
                default:
                    // The user has selected a command that requires them to log in.
                    await dc.ContinueAsync(cancellationToken);
                    if (!turnContext.Responded)
                    {
                        await dc.BeginAsync("graphDialog", cancellationToken: cancellationToken);
                    }

                    break;
            }

            return dc;
        }

        /// <summary>
        /// Waterfall dialog step to process the command sent by the user.
        /// </summary>
        /// <param name="dc">A <see cref="DialogContext"/> provides context for the current dialog.</param>
        /// <param name="step">A <see cref="WaterfallStepContext"/> provides context for the current waterfall step.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the operation.</returns>
        private async Task<DialogTurnResult> ProcessStepAsync(DialogContext dc, WaterfallStepContext step, CancellationToken cancellationToken)
        {
            if (step.Result != null)
            {
                // We do not need to store the token in the bot.  When we need the token we can
                // Just send another prompt.  If the token is valid the user will not need to log back in
                // The token will be available in the Result property of the task.
                var tokenResponse = step.Result as TokenResponse;

                // If we have the token use the user it authenticated so we may use it to make API calls.
                if (tokenResponse?.Token != null)
                {
                    var parts = this._stateAccessors.CommandState.GetAsync(dc.Context, cancellationToken: cancellationToken).Result.Split(' ');

                    if (parts[0].ToLowerInvariant() == "me")
                    {
                        await OAuthHelpers.ListMeAsync(dc.Context, tokenResponse);
                        await this._stateAccessors.CommandState.DeleteAsync(dc.Context, cancellationToken);
                    }
                    else if (parts[0].ToLowerInvariant().StartsWith("send"))
                    {
                        await OAuthHelpers.SendMailAsync(dc.Context, tokenResponse, parts[1]);
                        await this._stateAccessors.CommandState.DeleteAsync(dc.Context, cancellationToken);
                    }
                    else if (parts[0].ToLowerInvariant().StartsWith("recent"))
                    {
                        await OAuthHelpers.ListRecentMailAsync(dc.Context, tokenResponse);
                        await this._stateAccessors.CommandState.DeleteAsync(dc.Context, cancellationToken);
                    }
                    else
                    {
                        await dc.Context.SendActivityAsync($"your token is: {tokenResponse.Token}", cancellationToken: cancellationToken);
                        await this._stateAccessors.CommandState.DeleteAsync(dc.Context, cancellationToken);
                    }
                }
            }
            else
            {
                await dc.Context.SendActivityAsync("We couldn't log you in. Please Try again later.", cancellationToken: cancellationToken);
            }

            return Dialog.EndOfTurn;
        }

        /// <summary>
        /// Waterfall step that will prompt the user to log in if they are not already.
        /// </summary>
        /// <param name="dc">A <see cref="DialogContext"/> provides context for the current dialog.</param>
        /// <param name="step">A <see cref="WaterfallStepContext"/> provides context for the current waterfall step.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the operation.</returns>
        private async Task<DialogTurnResult> PromptStepAsync(DialogContext dc, WaterfallStepContext step, CancellationToken cancellationToken)
        {
            var activity = dc.Context.Activity;

            // Set the context if the message is not the magic code.
            if (activity.Type == ActivityTypes.Message &&
                !Regex.IsMatch(activity.Text, @"(\d{6})"))
            {
                await this._stateAccessors.CommandState.SetAsync(dc.Context, activity.Text, cancellationToken);
            }

            return await dc.BeginAsync("loginPrompt", cancellationToken: cancellationToken);
        }
    }
}