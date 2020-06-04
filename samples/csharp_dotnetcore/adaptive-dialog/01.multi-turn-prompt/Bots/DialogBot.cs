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
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    // This IBot implementation can run any type of Dialog. The use of type parameterization is to allows multiple different bots
    // to be run at different endpoints within the same project. This can be achieved by defining distinct Controller types
    // each with dependency on distinct IBot types, this way ASP Dependency Injection can glue everything together without ambiguity.
    // The ConversationState is used by the Dialog system. The UserState isn't, however, it might have been used in a Dialog implementation,
    // and the requirement is that all BotState objects are saved at the end of a turn.
    public class DialogBot : ActivityHandler
    {
        protected readonly BotState ConversationState;
        protected readonly ILogger Logger;
        protected readonly BotState UserState;
        private DialogManager DialogManager;
        protected Dialog adaptiveDialog;

        public DialogBot(ConversationState conversationState, UserState userState, ILogger<DialogBot> logger)
        {
            ConversationState = conversationState;
            UserState = userState;
            Logger = logger;
            string[] paths = { ".", "Dialogs", $"RootDialog.lg" };
            string fullPath = Path.Combine(paths);
            // Create instance of adaptive dialog. 
            adaptiveDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
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

            DialogManager = new DialogManager(adaptiveDialog);
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            Logger.LogInformation("Running dialog with Activity.");
            await DialogManager.OnTurnAsync(turnContext, cancellationToken: cancellationToken).ConfigureAwait(false);
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
