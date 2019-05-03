// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace Microsoft.BotBuilderSamples
{
    public class RootDialog : ComponentDialog
    {
        public RootDialog()
            : base(nameof(RootDialog))
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
                    // Inputs will skip the prompt if the property (turn.cardChoice in this case) already has value.
                    // Since we are using RepeatDialog, we will set AlwaysPrompt property so we do not skip this prompt
                    // and end up in an infinite loop.
                    AlwaysPrompt = true,
                    Choices = new List<Choice>()
                    {
                        new Choice() { Value = "Cancel" },
                        new Choice() { Value = "All cards" },
                        new Choice() { Value = "Animation card" },
                        new Choice() { Value = "Audio card" },
                        new Choice() { Value = "Hero card" },
                        new Choice() { Value = "Signin card" },
                        new Choice() { Value = "Thumbnail card" },
                        new Choice() { Value = "Video card" },
                        new Choice() { Value = "Adaptive card" }
                    }
                },
                new SwitchCondition()
                {
                    Condition = "turn.cardChoice",
                    Cases = new List<Case>() {
                        // SendActivity supports full language generation resolution.
                        // See here to learn more about language generation
                        // https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/language-generation
                        new Case("'Adaptive card'",  new List<IDialog>() { new SendActivity("[AdativeCardRef]") } ),
                        new Case("'Animation card'", new List<IDialog>() { new SendActivity("[AnimationCard]") } ),
                        new Case("'Audio card'",     new List<IDialog>() { new SendActivity("[AudioCard]") } ),
                        new Case("'Hero card'",      new List<IDialog>() { new SendActivity("[HeroCard]") } ),
                        new Case("'Signin card'",    new List<IDialog>() { new SendActivity("[SigninCard]") } ),
                        new Case("'Thumbnail card'", new List<IDialog>() { new SendActivity("[ThumbnailCard]") } ),
                        new Case("'Video card'",     new List<IDialog>() { new SendActivity("[VideoCard]") } ),
                        new Case("'Cancel'",         new List<IDialog>() { new SendActivity("Sure."), new EndDialog() } ),
                    },
                    Default = new List<IDialog>()
                    {
                        new SendActivity("[AllCards]")
                    }
                },
                // Go back and repeat this dialog. User can choose 'cancel' to cancel the dialog.
                new RepeatDialog()
            };
        }
    }
}
