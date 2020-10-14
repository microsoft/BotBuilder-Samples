// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
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
                        new TextInput()
                        {
                            Prompt = new ActivityTemplate("${NamePrompt()}"),
                            Property = "user.Name",
                            AlwaysPrompt = true,
                            InvalidPrompt = new ActivityTemplate("${InvalidName()}"),
                            Validations = new List<BoolExpression>
                            {
                                new BoolExpression("length(this.value) > 0")
                            }
                        },
                        new SendActivity("${GreetWithName()}"),
                        new NumberInput()
                        {
                            Prompt = new ActivityTemplate("${AgePrompt()}"),
                            Property = "user.Age",
                            AlwaysPrompt = true,
                            InvalidPrompt = new ActivityTemplate("${InvalidAge()}"),
                            Validations = new List<BoolExpression>()
                            {
                                new BoolExpression("this.value >= 18"),
                                new BoolExpression("this.value <= 120")
                            }
                        },
                        new SendActivity("${AckAge()}"),
                        new DateTimeInput()
                        {
                            Prompt = new ActivityTemplate("${FlightPrompt()}"),
                            Property = "dialog.TravelDate",
                            AlwaysPrompt = true,
                            Validations = new List<BoolExpression>()
                            {
                                "isDefinite(this.value[0].timex)"
                            },
                            InvalidPrompt = new ActivityTemplate("${InvalidDate()}")
                        },
                        new SetProperty()
                        {
                            Property = "user.TravelDate",
                            Value = "=formatDateTime(dialog.TravelDate[0].Value, 'MM/dd/yyyy')"
                        },
                        new SendActivity("${SummaryInfo()}"),
                        new SendActivity("${ThankYouMessage()}"),
                        new SendActivity("${RestartPrompt()}")
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
