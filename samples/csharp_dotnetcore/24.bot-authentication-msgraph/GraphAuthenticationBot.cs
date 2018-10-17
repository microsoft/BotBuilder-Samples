// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This bot uses OAuth to log the user in. The OAuth provider being demonstrated
    /// here is Azure Active Directory v2.0 (AADv2). Once logged in,the bot uses the
    /// Microsoft Graph API to demonstrate making calls to authenticated services.
    /// Represents a bot that processes incoming activities.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service.  Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class GraphAuthenticationBot : IBot
    {
        // The connection name here must match the the one from
        // your Bot Channels Registration on the settings blade in Azure.
        private const string ConnectionSettingName = "";

        // Instructions for the user with information about commands that this bot may handle.
        private const string WelcomeText =
            @"You can type 'send <recipient_email>' to send an email, 'recent' to view recent unread mail
            'me' to see information about yourself, or 'help' to view the commands
            again. Any other text will display your token.";

        private readonly GraphAuthenticationBotAccessors _stateAccessors;
        private readonly DialogSet _dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphAuthenticationBot"/> class.
        /// </summary>
        /// <param name="accessors">State accessors for the bot.</param>
        public GraphAuthenticationBot(GraphAuthenticationBotAccessors accessors)
        {
            if (string.IsNullOrWhiteSpace(ConnectionSettingName))
            {
                throw new InvalidOperationException("ConnectionSettingName must be configured prior to running the bot.");
            }

            _stateAccessors = accessors ?? throw new ArgumentNullException(nameof(accessors));
            _dialogs = new DialogSet(_stateAccessors.ConversationDialogState);
            _dialogs.Add(OAuthHelpers.Prompt(ConnectionSettingName));
            _dialogs.Add(new ChoicePrompt("choicePrompt"));
            _dialogs.Add(new WaterfallDialog("graphDialog", new WaterfallStep[] { PromptStepAsync, ProcessStepAsync }));
        }

        /// <summary>
        /// This controls what happens when an <see cref="Activity"/> gets sent to the bot.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            DialogContext dc = null;

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    await ProcessInputAsync(turnContext, cancellationToken);
                    break;
                case ActivityTypes.Event:
                case ActivityTypes.Invoke:
                    // This handles the Microsoft Teams Invoke Activity sent when magic code is not used.
                    // See: https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/authentication/auth-oauth-card#getting-started-with-oauthcard-in-teams
                    // Manifest Schema Here: https://docs.microsoft.com/en-us/microsoftteams/platform/resources/schema/manifest-schema
                    // It also handles the Event Activity sent from The Emulator when the magic code is not used.
                    // See: https://blog.botframework.com/2018/08/28/testing-authentication-to-your-bot-using-the-bot-framework-emulator/

                    // Sanity check the activity type and channel Id.
                    if (turnContext.Activity.Type == ActivityTypes.Invoke && turnContext.Activity.ChannelId != "msteams")
                    {
                        throw new InvalidOperationException("The Invoke type is only valid onthe MSTeams channel.");
                    }

                    dc = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                    await dc.ContinueDialogAsync(cancellationToken);
                    if (!turnContext.Responded)
                    {
                        await dc.BeginDialogAsync("graphDialog", cancellationToken: cancellationToken);
                    }

                    break;
                case ActivityTypes.ConversationUpdate:
                    if (turnContext.Activity.MembersAdded != null)
                    {
                        await SendWelcomeMessageAsync(turnContext, cancellationToken);
                    }

                    break;
            }
        }

        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var reply = turnContext.Activity.CreateReply();
                    reply.Text = WelcomeText;
                    reply.Attachments = new List<Attachment> { CreateHeroCard(member.Id).ToAttachment() };
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Creates a <see cref="HeroCard"/> that is sent as a welcome message to the user.
        /// </summary>
        /// <param name="newUserName"> The name of the user.</param>
        /// <returns>A <see cref="HeroCard"/> the user can interact with.</returns>
        private static HeroCard CreateHeroCard(string newUserName)
        {
            var heroCard = new HeroCard($"Welcome {newUserName}", "OAuthBot")
            {
                Images = new List<CardImage>
                {
                    new CardImage(
                        "https://botframeworksamples.blob.core.windows.net/samples/aadlogo.png",
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
        /// Processes input and route to the appropriate step.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the operation.</returns>
        private async Task<DialogContext> ProcessInputAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var dc = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
            switch (turnContext.Activity.Text.ToLowerInvariant())
            {
                case "signout":
                case "logout":
                case "signoff":
                case "logoff":
                    // The bot adapter encapsulates the authentication processes and sends
                    // activities to from the Bot Connector Service.
                    var botAdapter = (BotFrameworkAdapter)turnContext.Adapter;
                    await botAdapter.SignOutUserAsync(turnContext, ConnectionSettingName, cancellationToken: cancellationToken);

                    // Let the user know they are signed out.
                    await turnContext.SendActivityAsync("You are now signed out.", cancellationToken: cancellationToken);
                    break;
                case "help":
                    await turnContext.SendActivityAsync(WelcomeText, cancellationToken: cancellationToken);
                    break;
                default:
                    // The user has input a command that has not been handled yet,
                    // begin the waterfall dialog to handle the input.
                    await dc.ContinueDialogAsync(cancellationToken);
                    if (!turnContext.Responded)
                    {
                        await dc.BeginDialogAsync("graphDialog", cancellationToken: cancellationToken);
                    }

                    break;
            }

            return dc;
        }

        /// <summary>
        /// Waterfall dialog step to process the command sent by the user.
        /// </summary>
        /// <param name="step">A <see cref="WaterfallStepContext"/> provides context for the current waterfall step.</param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the operation.</returns>
        private async Task<DialogTurnResult> ProcessStepAsync(WaterfallStepContext step, CancellationToken cancellationToken)
        {
            if (step.Result != null)
            {
                // We do not need to store the token in the bot. When we need the token we can
                // send another prompt. If the token is valid the user will not need to log back in.
                // The token will be available in the Result property of the task.
                var tokenResponse = step.Result as TokenResponse;

                // If we have the token use the user is authenticated so we may use it to make API calls.
                if (tokenResponse?.Token != null)
                {
                    var parts = _stateAccessors.CommandState.GetAsync(step.Context, () => string.Empty, cancellationToken: cancellationToken).Result.Split(' ');
                    string command = parts[0].ToLowerInvariant();

                    if (command == "me")
                    {
                        await OAuthHelpers.ListMeAsync(step.Context, tokenResponse);
                    }
                    else if (command.StartsWith("send"))
                    {
                        await OAuthHelpers.SendMailAsync(step.Context, tokenResponse, parts[1]);
                    }
                    else if (command.StartsWith("recent"))
                    {
                        await OAuthHelpers.ListRecentMailAsync(step.Context, tokenResponse);
                    }
                    else
                    {
                        await step.Context.SendActivityAsync($"Your token is: {tokenResponse.Token}", cancellationToken: cancellationToken);
                    }

                    await _stateAccessors.CommandState.DeleteAsync(step.Context, cancellationToken);
                }
            }
            else
            {
                await step.Context.SendActivityAsync("We couldn't log you in. Please try again later.", cancellationToken: cancellationToken);
            }

            return await step.EndDialogAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Waterfall step that will prompt the user to log in if they are not already.
        /// </summary>
        /// <param name="step">A <see cref="WaterfallStepContext"/> provides context for the current waterfall step.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the operation.</returns>
        private async Task<DialogTurnResult> PromptStepAsync(WaterfallStepContext step, CancellationToken cancellationToken)
        {
            var activity = step.Context.Activity;

            // Set the context if the message is not the magic code.
            if (activity.Type == ActivityTypes.Message &&
                !Regex.IsMatch(activity.Text, @"(\d{6})"))
            {
                await _stateAccessors.CommandState.SetAsync(step.Context, activity.Text, cancellationToken);
                await _stateAccessors.UserState.SaveChangesAsync(step.Context, cancellationToken: cancellationToken);
            }

            return await step.BeginDialogAsync("loginPrompt", cancellationToken: cancellationToken);
        }
    }
}
