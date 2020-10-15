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
using Microsoft.Recognizers.Text.NumberWithUnit.English;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class RootDialog : AdaptiveDialog
    {
        private readonly Templates _templates;

        public RootDialog() : base(nameof(RootDialog))
        {
            string[] paths = { ".", "Dialogs", $"RootDialog.lg" };
            var fullPath = Path.Combine(paths);
            _templates = Templates.ParseFile(fullPath);

            Triggers = new List<OnCondition>
            {
                // Add a rule to welcome user
                new OnConversationUpdateActivity()
                {
                    Actions = WelcomeUserSteps()
                },
                new OnUnknownIntent
                {
                    Actions = SetValuesAndEcho()
                }
            };

            Generator = new TemplateEngineLanguageGenerator(_templates);
        }

        private List<Dialog> SetValuesAndEcho()
        {
            return new List<Dialog>()
            {
                // If the customState objects are not present, create them.
                new IfCondition
                {
                    Condition = "user.customState == null || conversation.customState == null",
                    Actions = new List<Dialog>
                    {
                        new SetProperties
                        {
                            Assignments = new List<PropertyAssignment>
                            {
                                new PropertyAssignment
                                {
                                    Property = "user.customState",
                                    Value = "=json('{\"value\":0}')"
                                },
                                new PropertyAssignment
                                {
                                    Property = "conversation.customState",
                                    Value = "=json('{\"value\":0}')"
                                }
                            }
                        }
                    }
                },
                // Increment the custom state values
                new SetProperty
                {
                    Property = "user.customState.value",
                    Value = "=add(user.customState.value, 1)"
                },
                new SetProperty
                {
                    Property = "conversation.customState.value",
                    Value = "=add(conversation.customState.value, 1)"
                },
                // Finally, notify the user of the current values.
                new SendActivity("${EchoWithValuesMessage()}"),
            };
        }

        private static List<Dialog> WelcomeUserSteps()
        {
            return new List<Dialog>()
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
}
