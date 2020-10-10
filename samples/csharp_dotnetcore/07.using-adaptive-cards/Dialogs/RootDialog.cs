// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.LanguageGeneration;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class RootDialog : AdaptiveDialog
    {
        private Templates _templates;

        public RootDialog() : base(nameof(RootDialog))
        {
            string[] paths = { ".", "Dialogs", $"RootDialog.lg" };
            var fullPath = Path.Combine(paths);
            _templates = Templates.ParseFile(fullPath);

            Triggers = new List<OnCondition>
            {
                new OnConversationUpdateActivity()
                {
                    Actions = WelcomeUserSteps()
                },
                new OnUnknownIntent()
                {
                    Actions = new List<Dialog>()
                    {
                        new SendActivity("${AdaptiveCard()}"),
                        new SendActivity("${SeeAnotherCardPrompt()}")
                    }
                }
            };

            Generator = new TemplateEngineLanguageGenerator(_templates);
        }

        private static List<Dialog> WelcomeUserSteps() =>
            new List<Dialog>()
            {
                // Iterate through membersAdded list and greet user added to the conversation.
                new Foreach()
                {
                    ItemsProperty = "turn.activity.membersAdded",
                    Actions = new List<Dialog>()
                    {
                        new IfCondition()
                        {
                            Condition = "$foreach.value.name != turn.activity.recipient.name",
                            Actions = new List<Dialog>()
                            {
                                new SendActivity("${WelcomeMessage()}")
                            }
                        }
                    }
                }
            };
    }
}
