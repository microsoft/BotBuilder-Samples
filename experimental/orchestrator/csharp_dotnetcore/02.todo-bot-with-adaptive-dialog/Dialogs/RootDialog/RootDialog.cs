// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Builder.AI.Orchestrator;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class RootDialog : ComponentDialog
    {
        public RootDialog(IConfiguration configuration)
            : base(nameof(RootDialog))
        {
            string[] paths = { ".", "Dialogs", "RootDialog", "RootDialog.lg" };
            string fullPath = Path.Combine(paths);

            // Create instance of adaptive dialog. 
            var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                // Add a generator. This is how all Language Generation constructs specified for this dialog are resolved.
                Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(fullPath)),

                // Create a Orchestrator recognizer.
                // The recognizer is built using the intents, utterances defined in ./RootDialog.lu file
                Recognizer = CreateRecognizer(configuration),
                Triggers = new List<OnCondition>()
                {
                    // Add a rule to welcome user
                    new OnConversationUpdateActivity()
                    {
                        Actions = WelcomeUserSteps()
                    },

                    // Intent rules for the LUIS model. Each intent here corresponds to an intent defined in ./RootDialog.lu file
                    new OnIntent("Greeting")
                    {
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("${HelpRootDialog()}")
                        }
                    },
                    new OnIntent("AddItem")
                    { 
                        // LUIS returns a confidence score with intent classification. 
                        // Conditions are expressions. 
                        // This expression ensures that this trigger only fires if the confidence score for the 
                        // AddToDoDialog intent classification is at least 0.7
                        Condition = "#AddItem.Score >= 0.5",
                        Actions = new List<Dialog>()
                        {
                            new BeginDialog(nameof(AddToDoDialog))
                        }
                    },
                    new OnIntent("DeleteItem")
                    {
                        Condition = "#DeleteItem.Score >= 0.5",
                        Actions = new List<Dialog>()
                        {
                            new BeginDialog(nameof(DeleteToDoDialog))
                        }
                    },
                    new OnIntent("ViewItem")
                    {
                        Condition = "#ViewItem.Score >= 0.5",
                        Actions = new List<Dialog>()
                        {
                            new BeginDialog(nameof(ViewToDoDialog))
                        }
                    },
                    new OnIntent("GetUserProfile")
                    {
                        Condition = "#GetUserProfile.Score >= 0.5",
                        Actions = new List<Dialog>()
                        {
                             new BeginDialog(nameof(GetUserProfileDialog))
                        }
                    },

                    // Come back with LG template based readback for global help
                    new OnIntent("Help")
                    {
                        Condition = "#Help.Score >= 0.8",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("${HelpRootDialog()}"),
                        }
                    },

                    new OnChooseIntent()
                    {
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("Ambiguous!"),
                            new SetProperty()
                            {
                                Value = "=turn.recognized.candidates",
                                Property = "dialog.disambigCandidates"
                            },
                            new TextInput()
                            {
                                Property = "turn.disambigChoice",
                                Prompt = new ActivityTemplate("${DisambigIntents()}"),
                                AllowInterruptions = false
                            },
                            new IfCondition()
                            {
                                Condition = "turn.disambigChoice != 'None'",
                                Actions = new List<Dialog>()
                                {
                                    new EmitEvent()
                                    {
                                        EventName = AdaptiveEvents.RecognizedIntent,
                                        EventValue = "=dialog.disambigCandidates[turn.disambigChoice].result",
                                        BubbleEvent = true
                                    }
                                },
                                ElseActions = new List<Dialog>()
                                {
                                    new SendActivity("${NoDisambigReadBack()}")
                                }
                            }
                        }
                    },
                    new OnIntent("Cancel")           
                    { 
                        Condition = "#Cancel.Score >= 0.8",
                        Actions = new List<Dialog>() 
                        {
                            // Ask user for confirmation.
                            // This input will still use the recognizer and specifically the confirm list entity extraction.
                            new ConfirmInput()
                            {
                                Prompt = new ActivityTemplate("${Cancel.prompt()}"),
                                Property = "turn.confirm",
                                Value = "=@confirmation",

                                // Allow user to intrrupt this only if we did not get a value for confirmation.
                                AllowInterruptions = "!@confirmation"
                            },
                            new IfCondition()
                            {
                                Condition = "turn.confirm == true",
                                Actions = new List<Dialog>()
                                {
                                    // This is the global cancel in case a child dialog did not explicit handle cancel.
                                    new SendActivity("Cancelling all dialogs.."),

                                    // SendActivity supports full language generation resolution.
                                    // See here to learn more about language generation
                                    // https://aka.ms/language-generation
                                    new SendActivity("${WelcomeActions()}"),
                                    new CancelAllDialogs(),
                                },
                                ElseActions = new List<Dialog>()
                                {
                                    new SendActivity("${CancelCancelled()}"),
                                    new SendActivity("${WelcomeActions()}")
                                }
                            }
                        }
                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(rootDialog);

            // Add all child dialogS
            AddDialog(new AddToDoDialog(configuration));
            AddDialog(new DeleteToDoDialog(configuration));
            AddDialog(new ViewToDoDialog(configuration));
            AddDialog(new GetUserProfileDialog(configuration));

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }

        public static Recognizer CreateRecognizer(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration["orchestrator:ModelPath"]) || string.IsNullOrEmpty(configuration["orchestrator:SnapShotPaths:RootDialog"]))
            {
                throw new Exception("Your RootDialog Orchestrator application is not configured. Please see README.MD to set it up.");
            }

            return new OrchestratorAdaptiveRecognizer()
            {
                ModelPath = configuration["orchestrator:ModelPath"],
                SnapshotPath = configuration["orchestrator:SnapShotPaths:RootDialog"],
                DetectAmbiguousIntents = true,
                DisambiguationScoreThreshold = 0.10F
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
                        // Note: Some channels send two conversation update events - one for the Bot added to the conversation and another for user.
                        // Filter cases where the bot itself is the recipient of the message. 
                        new IfCondition()
                        {
                            Condition = "$foreach.value.name != turn.activity.recipient.name",
                            Actions = new List<Dialog>()
                            {
                                new SendActivity("${IntroMessage()}"),

                                // Initialize global properties for the user.
                                new SetProperty()
                                {
                                    Property = "user.lists",
                                    Value = "={todo : [], grocery : [], shopping : []}"
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
