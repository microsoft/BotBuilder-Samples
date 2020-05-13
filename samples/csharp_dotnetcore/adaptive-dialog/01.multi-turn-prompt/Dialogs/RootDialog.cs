using System;
using System.Collections.Generic;
using System.IO;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;

namespace Microsoft.BotBuilderSamples
{
    public class RootDialog : ComponentDialog
    {
        public RootDialog()
            : base(nameof(RootDialog))
        {
            string[] paths = { ".", "Dialogs", $"{nameof(RootDialog)}.lg" };
            string fullPath = Path.Combine(paths);
            // Create instance of adaptive dialog. 
            var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                // These steps are executed when this Adaptive Dialog begins
                Triggers = new List<OnCondition>()
                {
                    // Add a rule to welcome user
                    new OnConversationUpdateActivity()
                    {
                        Actions = WelcomeUserSteps()
                    },

                    // Respond to user on message activity
                    new OnUnknownIntent()
                    {
                        Actions = OnBeginDialogSteps()
                    }
                },
                Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(fullPath))
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(rootDialog);
            
            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
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
                        // Note: Some channels send two conversation update events - one for the Bot added to the conversation and another for user.
                        // Filter cases where the bot itself is the recipient of the message. 
                        new IfCondition()
                        {
                            Condition = "$foreach.value.name != turn.activity.recipient.name",
                            Actions = new List<Dialog>()
                            {
                                new SendActivity("Hello, I'm the multi-turn prompt bot. Please send a message to get started!")
                            }
                        }
                    }
                }
            };

        }

        private static List<Dialog> OnBeginDialogSteps()
        {
            return new List<Dialog>()
            {
                // Ask for user's age and set it in user.userProfile scope.
                new TextInput()
                {
                    Prompt = new ActivityTemplate("${ModeOfTransportPrompt()}"),
                    // Set the output of the text input to this property in memory.
                    Property = "user.userProfile.Transport"
                },
                new TextInput()
                {
                    Prompt = new ActivityTemplate("${AskForName()}"),
                    Property = "user.userProfile.Name"
                },
                // SendActivity supports full language generation resolution.
                // See here to learn more about language generation
                // https://aka.ms/language-generation
                new SendActivity("${AckName()}"),
                new ConfirmInput()
                {
                    Prompt = new ActivityTemplate("${AgeConfirmPrompt()}"),
                    Property = "turn.ageConfirmation"
                },
                new IfCondition()
                {
                    // All conditions are expressed using adaptive expressions.
                    // See https://aka.ms/adaptive-expressions to learn more
                    Condition = "turn.ageConfirmation == true",
                    Actions = new List<Dialog>()
                    {
                         new NumberInput()
                         {
                             Prompt = new ActivityTemplate("${AskForAge()}"),
                             Property = "user.userProfile.Age",
                             // Add validations
                             Validations = new List<BoolExpression>()
                             {
                                 // Age must be greater than or equal 1
                                 "int(this.value) >= 1",
                                 // Age must be less than 150
                                 "int(this.value) < 150"
                             },
                             InvalidPrompt = new ActivityTemplate("${AskForAge.invalid()}"),
                             UnrecognizedPrompt = new ActivityTemplate("${AskForAge.unRecognized()}")
                         },
                         new SendActivity("${UserAgeReadBack()}")
                    },
                    ElseActions = new List<Dialog>()
                    {
                        new SendActivity("${NoName()}") 
                    }
                },
                new ConfirmInput()
                {
                    Prompt = new ActivityTemplate("${ConfirmPrompt()}"),
                    Property = "turn.finalConfirmation"
                },
                // Use LG template to come back with the final read out.
                // This LG template is a great example of what logic can be wrapped up in LG sub-system.
                new SendActivity("${FinalUserProfileReadOut()}"), // examines turn.finalConfirmation
                new EndDialog()
            };
        }
    }
}
