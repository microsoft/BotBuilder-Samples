// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.LanguageGeneration;

namespace Microsoft.BotBuilderSamples
{
    public class RootDialog : AdaptiveDialog
    {
        private readonly ActivityLog _log;

        public RootDialog(ActivityLog log) : base(nameof(RootDialog))
        {
            this._log = log;

            string[] paths = { ".", "Dialogs", $"RootDialog.lg" };
            var fullPath = Path.Combine(paths);
            var templates = Templates.ParseFile(fullPath);
            Generator = new TemplateEngineLanguageGenerator(templates);

            AutoEndDialog = false;

            // These steps are executed when this Adaptive Dialog begins
            Triggers = new List<OnCondition>
            {
                // Setup the trigger to handle Message Reactions.
                new OnMessageReactionActivity
                {
                    Actions = new List<Dialog>
                    {
                        new CodeAction(FindActivityAsync),
                        new IfCondition
                        {
                            Condition = "turn.activity.reactionsAdded != null",
                            Actions = new List<Dialog>
                            {
                                // Iterate through reactionsAdded list and process.
                                new Foreach
                                {
                                    ItemsProperty = "turn.activity.reactionsAdded",
                                    Actions = new List<Dialog>
                                    {
                                        new SendActivity("${ReactionAddedMessage()}")
                                    }
                                }
                            }
                        },
                        new IfCondition
                        {
                            Condition = "turn.activity.reactionsRemoved != null",
                            Actions = new List<Dialog>
                            {
                                // Iterate through reactionsRemoved list and process.
                                new Foreach
                                {
                                    ItemsProperty = "turn.activity.reactionsRemoved",
                                    Actions = new List<Dialog>
                                    {
                                        new SendActivity("${ReactionRemovedMessage()}")
                                    }
                                }
                            }
                        }
                    }
                },
                // Add a trigger to welcome user
                new OnConversationUpdateActivity
                {
                    Actions = WelcomeUserSteps()
                },
                // Respond to user on message activity
                new OnUnknownIntent
                {   
                    Actions = new List<Dialog>
                    {
                        new SendActivity("${EchoMessage()}"),
                    }
                },
            };
            
        }

        private async Task<DialogTurnResult> FindActivityAsync(DialogContext dc, object arg2)
        {
            // The ReplyToId property of the inbound MessageReaction Activity will correspond to a Message Activity which
            // had previously been sent from this bot.
            var activity = await _log.FindAsync(dc.Context.Activity.ReplyToId);
            if (activity == null)
            {
                // If we had sent the message from the error handler we wouldn't have recorded the Activity Id and so we
                // shouldn't expect to see it in the log.
                dc.State.SetValue("turn.foundActivity", MessageFactory.Text($"Activity {dc.Context.Activity.ReplyToId} not found in the log."));
            }
            else
            {
                dc.State.SetValue("turn.foundActivity", activity);
            }

            await dc.EndDialogAsync().ConfigureAwait(false);
            return new DialogTurnResult(DialogTurnStatus.Complete);
        }

        private List<Dialog> WelcomeUserSteps()
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
                                new SendActivity("${WelcomeMessage()}"),
                            }
                        }
                    }
                }
            };
        }
    }
}
