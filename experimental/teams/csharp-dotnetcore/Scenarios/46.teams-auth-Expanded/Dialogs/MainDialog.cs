// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    public class MainDialog : LogoutDialog
    {
        protected readonly ILogger Logger;
        private const string OAuth_Prompt = "OAuth Prompt";
        private const string OAuth_Card = "OAuth Card";
        private const string SignIn_Card = "Sign In Card";
        private List<string> AuthOptions = new List<string> { OAuth_Prompt, OAuth_Card, SignIn_Card };

        public MainDialog(IConfiguration configuration, ILogger<MainDialog> logger)
            : base(nameof(MainDialog), configuration["ConnectionName"])
        {
            Logger = logger;
            
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            AddDialog(new OAuthPrompt(
                nameof(OAuthPrompt),
                new OAuthPromptSettings
                {
                    ConnectionName = ConnectionName,
                    Text = "Please Sign In",
                    Title = "Sign In",
                    Timeout = 300000, // User has 5 minutes to login (1000 * 60 * 5)
                }));

            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ChooseAuthTypePromptStepAsync,
                PromptStepAsync,
                LoginStepAsync,
                DisplayTokenPhase1Async,
                DisplayTokenPhase2Async,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ChooseAuthTypePromptStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Please choose the type of Auth Prompt to test."),
                    Choices = ChoiceFactory.ToChoices(AuthOptions),
                }, cancellationToken);
        }

        private async Task<DialogTurnResult> PromptStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            
            var attachments = new List<Attachment>();
            var reply = MessageFactory.Attachment(attachments);
            // Hack to remove \n, which is currently appended to the end of a message when the bot is @mentioned
            stepContext.Context.Activity.RemoveRecipientMention().Replace("\n", string.Empty).Trim();
            var text = stepContext.Context.Activity.Text;

            switch (text)
            {
                case OAuth_Card:
                    attachments.Add(new Attachment
                    {
                        ContentType = OAuthCard.ContentType,
                        Content = new OAuthCard
                        {
                            Text = "OAuth Card",
                            ConnectionName = ConnectionName,
                            Buttons = new[]
                        {
                            new CardAction
                            {
                                Title = "Sign In",
                                Text = "Sign In",
                                Type = ActionTypes.Signin,
                            },
                        },
                        },
                    });

                    await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(attachments), cancellationToken);
                    return new DialogTurnResult(DialogTurnStatus.Waiting);

                case OAuth_Prompt:
                    return await stepContext.BeginDialogAsync(nameof(OAuthPrompt), null, cancellationToken);

                case SignIn_Card:

                    var link = await (stepContext.Context.Adapter as IUserTokenProvider).GetOauthSignInLinkAsync(stepContext.Context, ConnectionName, cancellationToken).ConfigureAwait(false);
                    attachments.Add(new Attachment
                    {
                        ContentType = SigninCard.ContentType,
                        Content = new SigninCard
                        {
                            Text = "Sign In Card",
                            Buttons = new[]
                                    {
                                new CardAction
                                {
                                    Title = "Sign In",
                                    Value = link,
                                    Type = ActionTypes.Signin,
                                },
                            },
                        },
                    });

                    await stepContext.Context.SendActivityAsync(MessageFactory.Attachment(attachments), cancellationToken);
                    return new DialogTurnResult(DialogTurnStatus.Waiting);

                default:
                    // invalid (user did not select a valid option)
                    await stepContext.RepromptDialogAsync(cancellationToken);
                    //await stepContext.Context.SendActivityAsync(MessageFactory.Text("Invalid.  Please choose one of the options"), cancellationToken);
                    return new DialogTurnResult(DialogTurnStatus.Waiting);                    
            }
        }

        private async Task<DialogTurnResult> LoginStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Hack for SignIn card (see LogoutDialog OnContinueDialogAsync)
            if (stepContext.Context.Activity.Text == base.ConnectionName)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("You are now logged in."), cancellationToken);
                return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Would you like to view your token?") }, cancellationToken);
            }
            // Get the token from the previous step. Note that we could also have gotten the
            // token directly from the prompt itself. There is an example of this in the next method.
            var tokenResponse = (TokenResponse)stepContext.Result;
            if (tokenResponse != null)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("You are now logged in."), cancellationToken);
                return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("Would you like to view your token?") }, cancellationToken);
            }

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Login was not successful please try again."), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> DisplayTokenPhase1Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Thank you."), cancellationToken);

            var result = (bool)stepContext.Result;
            if (result)
            {
                // Call the prompt again because we need the token. The reasons for this are:
                // 1. If the user is already logged in we do not need to store the token locally in the bot and worry
                // about refreshing it. We can always just call the prompt again to get the token.
                // 2. We never know how long it will take a user to respond. By the time the
                // user responds the token may have expired. The user would then be prompted to login again.
                //
                // There is no reason to store the token locally in the bot because we can always just call
                // the OAuth prompt to get the token or get a new token if needed.
                return await stepContext.BeginDialogAsync(nameof(OAuthPrompt), cancellationToken: cancellationToken);
            }

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> DisplayTokenPhase2Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var tokenResponse = (TokenResponse)stepContext.Result;
            if (tokenResponse != null)
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text($"Here is your token {tokenResponse.Token}"), cancellationToken);
            }

            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        public static SigninCard GetSigninCard()
        {
            var signinCard = new SigninCard
            {
                Text = "Sign-in Card",
                Buttons = new List<CardAction> { new CardAction(ActionTypes.Signin, "Sign-in", value: "https://login.microsoftonline.com/") },
            };

            return signinCard;
        }
    }
}
