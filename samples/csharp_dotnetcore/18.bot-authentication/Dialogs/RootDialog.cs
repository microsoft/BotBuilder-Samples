// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
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
                new OAuthInput
                {
                    ConnectionName = _connectionName,
                    Text = new StringExpression(_templates.Evaluate("OAuthText") as string),
                    Title = new StringExpression(_templates.Evaluate("OAuthTitle") as string),
                    Timeout = 300000, // User has 5 minutes to login (1000 * 60 * 5)
                    Property = "user.tokenResponse",
                    AllowInterruptions = true,
                    InvalidPrompt = new ActivityTemplate("${InvalidPrompt()}"),
                },
                new IfCondition
                {
                    Condition = "user.tokenResponse == null",
                    Actions = new List<Dialog>
                    {
                        new SendActivity("${LoginFailedMessage()}"),
                    },
                    ElseActions = new List<Dialog>
                    {
                        new SendActivity("${LoginSucceededMessage()}"),
                        new ConfirmInput
                        {
                            Prompt = new ActivityTemplate("${ViewTokenConfirmation()}"),
                            Property = "turn.viewTokenConfirmed",
                            AllowInterruptions = true,
                        },
                        new IfCondition
                        {
                            Condition = "turn.viewTokenConfirmed",
                            Actions = new List<Dialog>
                            {
                                new SendActivity("${ViewToken()}"),
                            },
                            ElseActions = new List<Dialog>
                            {
                                new SendActivity("${Thanks()}"),
                            }
                        }
                    }
                },
            };
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
