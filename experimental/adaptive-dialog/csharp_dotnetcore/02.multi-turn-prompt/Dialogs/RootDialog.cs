using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    public class RootDialog : ComponentDialog
    {
        public RootDialog()
            : base(nameof(RootDialog))
        {
            string[] paths = { ".", "Dialogs", "RootDialog.LG" };
            string fullPath = Path.Combine(paths);
            // Create instance of adaptive dialog. 
            var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                // These steps are executed when this Adaptive Dialog begins
                Rules = new List<IRule>()
                {
                    // Add a rule to welcome user
                    new ConversationUpdateActivityRule()
                    {
                        Steps = WelcomeUserSteps()
                    },

                    // Respond to user on message activity
                    new EventRule()
                    {
                        Events = new List<String> ()
                        {
                            AdaptiveEvents.BeginDialog
                        },
                        Constraint = $"turn.activity.type == '{ActivityTypes.Message}'",
                        Steps = OnBeginDialogSteps()
                    }
                },
                Generator = new TemplateEngineLanguageGenerator(new TemplateEngine().AddFile(fullPath))
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(rootDialog);
            
            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }

        private static List<IDialog> WelcomeUserSteps()
        {
            return new List<IDialog>()
            {
                // Iterate through membersAdded list and greet user added to the conversation.
                new Foreach()
                {
                    ListProperty = "turn.activity.membersAdded",
                    ValueProperty = "turn.memberAdded",
                    Steps = new List<IDialog>()
                    {
                        // Note: Some channels send two conversation update events - one for the Bot added to the conversation and another for user.
                        // Filter cases where the bot itself is the recipient of the message. 
                        new IfCondition()
                        {
                            Condition = "turn.memberAdded.name != turn.activity.recipient.name",
                            Steps = new List<IDialog>()
                            {
                                new SendActivity("Hello, I'm the cards bot. Please send a message to get started!")
                            }
                        }
                    }
                }
            };

        }

        private static List<IDialog> OnBeginDialogSteps()
        {
            return new List<IDialog>()
            {
                // Ask for user's age and set it in user.userProfile scope.
                new TextInput()
                {
                    Prompt = new ActivityTemplate("[ModeOfTransportPrompt]"),
                    // Set the output of the text input to this property in memory.
                    Property = "user.userProfile.Transport"
                },
                new TextInput()
                {
                    Prompt = new ActivityTemplate("[AskForName]"),
                    Property = "user.userProfile.Name"
                },
                // SendActivity supports full language generation resolution.
                // See here to learn more about language generation
                // https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/language-generation
                new SendActivity("[AckName]"),
                new ConfirmInput()
                {
                    Prompt = new ActivityTemplate("[AgeConfirmPrompt]"),
                    Property = "turn.ageConfirmation"
                },
                new IfCondition()
                {
                    // All conditions are expressed using the common expression language.
                    // See https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/common-expression-language to learn more
                    Condition = "turn.ageConfirmation == true",
                    Steps = new List<IDialog>()
                    {
                         new NumberInput()
                         {
                             Prompt = new ActivityTemplate("[AskForAge]"),
                             Property = "user.userProfile.Age",
                             // Set min and max constraints for age.
                             //MinValue = 1,
                             //MaxValue = 150,
                             // Specify a retry prompt if user input does not meet min or max constraint.
                             //RetryPrompt = new ActivityTemplate("[AskForAge.reprompt]")

                         },
                         new SendActivity("[UserAgeReadBack]")
                    },
                    ElseSteps = new List<IDialog>()
                    {
                        new SendActivity("[NoName]") 
                    }
                },
                new ConfirmInput()
                {
                    Prompt = new ActivityTemplate("[ConfirmPrompt]"),
                    Property = "turn.finalConfirmation"
                },
                // Use LG template to come back with the final read out.
                // This LG template is a great example of what logic can be wrapped up in LG sub-system.
                new SendActivity("[FinalUserProfileReadOut]"), // examines turn.finalConfirmation
                new EndDialog()
            };
        }
    }
}
