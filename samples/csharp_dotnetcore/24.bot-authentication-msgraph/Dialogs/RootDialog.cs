// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class RootDialog : AdaptiveDialog
    {
        string _connectionName;
        Templates _templates;

        public RootDialog(IConfiguration configuration)
            : base(nameof(RootDialog))
        {
            // Retrieve the OAuth Connection Name, to use for logging in and logging out
            _connectionName = configuration["ConnectionName"];

            string[] paths = { ".", "Dialogs", $"{nameof(RootDialog)}.lg" };
            string fullPath = Path.Combine(paths);
            _templates = Templates.ParseFile(fullPath);

            // Add a Recognizer to watch for when the user wishes to logout
            Recognizer = new RegexRecognizer
            {
                Intents = new List<IntentPattern>
                {
                   new IntentPattern("Logout", "(?i)logout")
                },
            };

            Triggers = new List<OnCondition>()
            {
                // Add a trigger to welcome user.
                new OnConversationUpdateActivity
                {
                    Actions = WelcomeUserSteps()
                },
                // Trigger for the Logout intent from recognizer above.
                new OnIntent
                {
                    Intent = "Logout",
                    Actions = new List<Dialog>
                    {
                        // Logout through a CodeAction, which calls adapter.SignOutUserAsync
                        new CodeAction(LogoutAsync),
                        // The user has been signed out, so cannot proceed further.
                        new CancelAllDialogs()
                    }
                },
                // Respond to user on message activity
                new OnUnknownIntent
                {
                    Actions = DialogActions()
                }
            };

            Generator = new TemplateEngineLanguageGenerator(_templates);
        }

        private async Task<DialogTurnResult> LogoutAsync(DialogContext dc, System.Object options)
        {
            // The bot adapter encapsulates the authentication code.
            var botAdapter = (BotFrameworkAdapter)dc.Context.Adapter;
            await botAdapter.SignOutUserAsync(dc.Context, _connectionName, null, CancellationToken.None).ConfigureAwait(false);
            await dc.Context.SendActivityAsync(MessageFactory.Text(_templates.Evaluate("LogoutMessage") as string), CancellationToken.None).ConfigureAwait(false);
            return await dc.EndDialogAsync(options);
        }

        private List<Dialog> DialogActions()
        {
            return new List<Dialog>
            {
                // Setup OAuthInput to prompt the user to sign in, and populate the
                // user.tokenResponse property with the tokenResponse object.  The token
                // can later be used to access apis on behalf of the user.
                // OAuthInput prompts can be used in multiple places, immediately before
                // other dialogs which require the token to access resources.
                // If the user had recently signed in, OauthInput will retrieve the token
                // from the Bot Authentication Service and not prompt the user to sign-in again.
                new OAuthInput
                {
                    ConnectionName = _connectionName,
                    Text = _templates.Evaluate("OAuthText") as string,
                    Title = _templates.Evaluate("OAuthTitle") as string,
                    Timeout = 300000, // User has 5 minutes to login (1000 * 60 * 5)
                    Property = "dialog.tokenResponse",
                    AllowInterruptions = true,
                    InvalidPrompt = new ActivityTemplate("${InvalidPrompt()}"),
                },
                new IfCondition
                {
                    // dialog.tokenResponse will only be missing if the login failed.
                    Condition = "dialog.tokenResponse == null",
                    Actions = new List<Dialog>
                    {
                        new SendActivity("${LoginFailedMessage()}"),
                    },
                    ElseActions = new List<Dialog>
                    {
                        new SendActivity("${LoginSucceededMessage()}"),
                        new SendActivity("${WhatWouldYouLikeToDoMessage()}"),
                        new EndTurn(),
                        // Call the prompt again because we need the token. The reasons for this are:
                        // 1. If the user is already logged in we do not need to store the token locally in the bot 
                        // and worry about refreshing it. We can always just call the prompt again to get the token.
                        // 2. We never know how long it will take a user to respond. By the time the
                        // user responds the token may have expired. The user would then be prompted to login again.
                        //
                        // There is no reason to store the token locally in the bot because we can always just call
                        // the OAuth prompt to get the token or get a new token if needed.
                        new OAuthInput
                        {
                            ConnectionName = _connectionName,
                            Text = _templates.Evaluate("OAuthText") as string,
                            Title = _templates.Evaluate("OAuthTitle") as string,
                            Timeout = 300000, // User has 5 minutes to login (1000 * 60 * 5)
                            Property = "dialog.tokenResponse2",
                            AllowInterruptions = true,
                            InvalidPrompt = new ActivityTemplate("${InvalidPrompt()}"),
                        },
                        new SwitchCondition
                        {
                            Condition = "trim(turn.activity.text)",
                            Cases = new List<Case>()
                            {
                                // Use the token with Graph to retrieve the user's name or email address.
                                new Case("me",  new List<Dialog>() { new CodeAction(ListMeAsync)}),
                                new Case("email", new List<Dialog>() { new CodeAction(ListEmailAddressAsync)}),
                            },
                            Default = new List<Dialog>()
                            {
                                new SendActivity("${ViewTokenResponse2()}"),
                            }
                        },
                    }
                },
            };
        }

        private async Task<DialogTurnResult> ListEmailAddressAsync(DialogContext dc, object options)
        {
            var tokenResponse = dc.State.GetValue<TokenResponse>("dialog.tokenResponse2", ()=> null);
            await OAuthHelpers.ListEmailAddressAsync(dc.Context, tokenResponse);
            return await dc.EndDialogAsync(options);
        }

        private async Task<DialogTurnResult> ListMeAsync(DialogContext dc, object options)
        {
            var tokenResponse = dc.State.GetValue<TokenResponse>("dialog.tokenResponse2", () => null);
            await OAuthHelpers.ListMeAsync(dc.Context, tokenResponse);
            return await dc.EndDialogAsync(options);
        }

        private static List<Dialog> WelcomeUserSteps()
        {
            return new List<Dialog>
            {
                // Iterate through membersAdded list and greet user added to the conversation.
                new Foreach
                {
                    ItemsProperty = "turn.activity.membersAdded",
                    Actions = new List<Dialog>
                    {
                        // Note: Some channels send two conversation update events - one for the Bot added to the conversation and another for user.
                        // Filter cases where the bot itself is the recipient of the message. 
                        new IfCondition
                        {
                            Condition = "$foreach.value.name != turn.activity.recipient.name",
                            Actions = new List<Dialog>
                            {
                                new SendActivity("${WelcomeMessage()}")
                            }
                        }
                    }
                }
            };
        }
    }
}
