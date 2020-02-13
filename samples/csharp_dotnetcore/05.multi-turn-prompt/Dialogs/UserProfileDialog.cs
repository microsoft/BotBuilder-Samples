// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using System;

namespace Microsoft.BotBuilderSamples
{
    public class UserProfileDialog : ComponentDialog
    {
        public UserProfileDialog()
            : base(nameof(UserProfileDialog))
        {
            // Create instance of adaptive dialog. 
            var userProfileDialog = new AdaptiveDialog(nameof(AdaptiveDialog));

            // Add LG
            string[] lgFile = { ".", "Dialogs", "UserProfileDialog.lg" };
            string lgFilePath = Path.Combine(lgFile);
            var lgEngine = new TemplateEngine().AddFile(lgFilePath);
            userProfileDialog.Generator = new TemplateEngineLanguageGenerator(lgEngine);

            // Create triggers
            var welcomeUserTrigger = new OnConversationUpdateActivity(WelcomeUserSteps());
            var dialogActions = new OnUnknownIntent(OnBeginDialogSteps());

            // Add triggers
            userProfileDialog.Triggers.Add(welcomeUserTrigger);
            userProfileDialog.Triggers.Add(dialogActions);

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(userProfileDialog);
            
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
                    Prompt = new ActivityTemplate("@{ModeOfTransportPrompt()}"),
                    // Set the output of the text input to this property in memory.
                    Property = "user.userProfile.Transport"
                },
                new TextInput()
                {
                    Prompt = new ActivityTemplate("@{AskForName()}"),
                    Property = "user.userProfile.Name"
                },
                // SendActivity supports full language generation resolution.
                // See here to learn more about language generation
                // https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/language-generation
                new SendActivity("@{AckName()}"),
                new ConfirmInput()
                {
                    Prompt = new ActivityTemplate("@{AgeConfirmPrompt()}"),
                    Property = "turn.ageConfirmation"
                },
                new IfCondition()
                {
                    // All conditions are expressed using the common expression language.
                    // See https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/common-expression-language to learn more
                    Condition = "turn.ageConfirmation == true",
                    Actions = new List<Dialog>()
                    {
                         new NumberInput()
                         {
                             Prompt = new ActivityTemplate("@{AskForAge()}"),
                             Property = "user.userProfile.Age",
                             // Add validations
                             Validations = new List<String>()
                             {
                                 // Age must be greater than or equal 1
                                 "int(this.value) >= 1",
                                 // Age must be less than 150
                                 "int(this.value) < 150"
                             },
                             InvalidPrompt = new ActivityTemplate("@{AskForAge.invalid()}"),
                             UnrecognizedPrompt = new ActivityTemplate("@{AskForAge.unRecognized()}")
                         },
                         new SendActivity("@{UserAgeReadBack()}")
                    },
                    ElseActions = new List<Dialog>()
                    {
                        new SendActivity("@{NoName()}") 
                    }
                },
                new ConfirmInput()
                {
                    Prompt = new ActivityTemplate("@{ConfirmPrompt()}"),
                    Property = "turn.finalConfirmation"
                },
                // Use LG template to come back with the final read out.
                // This LG template is a great example of what logic can be wrapped up in LG sub-system.
                new SendActivity("@{FinalUserProfileReadOut()}"), // examines turn.finalConfirmation
                new EndDialog()
            };
        }
    }
}
