// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Expressions.Parser;

namespace Microsoft.BotBuilderSamples
{
    public class MainAdaptiveDialog : ComponentDialog
    {
        public MainAdaptiveDialog()
            : base(nameof(MainAdaptiveDialog))
        {

            // Create instance of adaptive dialog. 
            var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                // These steps are executed when this Adaptive Dialog begins
                Steps = OnBeginDialogSteps(),
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(rootDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }
        private static List<IDialog> OnBeginDialogSteps()
        {
            return new List<IDialog>()
            {
                // Present choices for card types
                new ChoiceInput()
                {
                    Prompt = new ActivityTemplate("[CardChoice]"),
                    Property = "turn.cardChoice",
                    Style = ListStyle.Auto,
                    Choices = new List<Choice>()
                    {
                        new Choice() { Value = "Adaptive card" },
                        new Choice() { Value = "Animation card" },
                        new Choice() { Value = "Audio card" },
                        new Choice() { Value = "Hero card" },
                        new Choice() { Value = "Receipt card" },
                        new Choice() { Value = "Signin card" },
                        new Choice() { Value = "Thumbnail card" },
                        new Choice() { Value = "Video card" },
                        new Choice() { Value = "All cards" }
                    }
                },
                new SwitchCondition()
                {

                },
                new RepeatDialog()
            };
        }
    }
}
