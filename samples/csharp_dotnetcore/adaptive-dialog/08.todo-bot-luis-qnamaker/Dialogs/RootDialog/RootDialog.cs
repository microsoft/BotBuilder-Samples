// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using System.IO;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.AI.QnA.Recognizers;
using AdaptiveExpressions;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Microsoft.Bot.Builder;

namespace Microsoft.BotBuilderSamples
{
    public class RootDialog : ComponentDialog
    {
        private static IConfiguration Configuration;
        private AdaptiveDialog _rootDialog;

        public RootDialog(IConfiguration configuration)
            : base(nameof(RootDialog))
        {
            Configuration = configuration;
            string[] paths = { ".", "Dialogs", "RootDialog", "RootDialog.lg" };
            string fullPath = Path.Combine(paths);
            // Create instance of adaptive dialog. 
            _rootDialog = new AdaptiveDialog(nameof(RootDialog))
            {
                // Add a generator. This is how all Language Generation constructs specified for this dialog are resolved.
                Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(fullPath)),
                // Create a LUIS recognizer.
                // The recognizer is built using the intents, utterances, patterns and entities defined in ./RootDialog.lu file
                Recognizer = CreateCrossTrainedRecognizer(configuration),
                Triggers = new List<OnCondition>()
                {
                    // Add a rule to welcome user
                    new OnConversationUpdateActivity()
                    {
                        Actions = WelcomeUserSteps(),
                    },

                    // Intent rules for the LUIS model. Each intent here corresponds to an intent defined in ./Dialogs/Resources/ToDoBot.lu file
                    new OnIntent("Greeting")
                    {
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("${HelpRootDialog()}"),
                        },
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
                            new BeginDialog(nameof(AddToDoDialog)),
                        },
                    },
                    new OnIntent("DeleteItem") 
                    {
                        Condition = "#DeleteItem.Score >= 0.5",
                        Actions = new List<Dialog>() 
                        {
                            new BeginDialog(nameof(DeleteToDoDialog)),
                        },
                    },
                    new OnIntent("ViewItem")
                    {
                        Condition = "#ViewItem.Score >= 0.5",
                        Actions = new List<Dialog>() 
                        {
                            new BeginDialog(nameof(ViewToDoDialog)),
                        },
                    },
                    new OnIntent("GetUserProfile")
                    {
                        Condition = "#GetUserProfile.Score >= 0.5",
                        Actions = new List<Dialog>()
                        {
                             new BeginDialog(nameof(GetUserProfileDialog)),
                        },
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
                                AllowInterruptions = "!@confirmation",
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
                                    new SendActivity("${WelcomeActions()}"),
                                },
                            },
                        },
                    },
                    // Help and chitchat is handled by qna
                    new OnQnAMatch
                    {
                        Actions = new List<Dialog>()
                        {
                            new CodeAction(this.ResolveAndSendQnAAnswer)
                        },
                    },
                    // This trigger fires when the recognizers do not agree on who should win.
                    // This enables you to write simple rules to pick a winner or ask the user for disambiguation.
                    new OnChooseIntent()
                    {
                        Actions = new List<Dialog>()
                        {
                            new SetProperties()
                            {
                                Assignments = new List<PropertyAssignment>()
                                {
                                    // Get the recognition result values for each recognizer configured on this dialog by ID.
                                    new PropertyAssignment()
                                    {
                                        Property = "dialog.luisResult",
                                        Value = $"=jPath(turn.recognized, \"$.candidates[?(@.id == 'LUIS_{nameof(RootDialog)}')]\")"
                                    },
                                    new PropertyAssignment()
                                    {
                                        Property = "dialog.qnaResult",
                                        Value = $"=jPath(turn.recognized, \"$.candidates[?(@.id == 'QnA_{nameof(RootDialog)}')]\")"
                                    },
                                }
                            },

                            // Rules to determine winner before disambiguation
                            // Rule 1: High confidence result from LUIS and Low confidence result from QnA => Pick LUIS result
                            new IfCondition()
                            {
                                Condition = "dialog.luisResult.score >= 0.9 && dialog.qnaResult.score <= 0.5",
                                Actions = new List<Dialog>()
                                {
                                    // By Emitting a recognized intent event with the recognition result from LUIS, adaptive dialog
                                    // will evaluate all triggers with that recognition result.
                                    new EmitEvent()
                                    {
                                        EventName = AdaptiveEvents.RecognizedIntent,
                                        EventValue = "=dialog.luisResult.result"
                                    },
                                    new BreakLoop()
                                }
                            },

                            // Rule 2: High confidence result from QnA, Low confidence result from LUIS => Pick QnA result
                            new IfCondition()
                            {
                                Condition = "dialog.luisResult.score <= 0.5 && dialog.qnaResult.score >= 0.9",
                                Actions = new List<Dialog>()
                                {
                                    new EmitEvent()
                                    {
                                        EventName = AdaptiveEvents.RecognizedIntent,
                                        EventValue = "=dialog.qnaResult.result"
                                    },
                                    new BreakLoop()
                                }
                            },

                            // Rule 3: QnA has exact match (>=0.95) => Pick QnA result
                            new IfCondition()
                            {
                                Condition = "dialog.qnaResult.score >= 0.95",
                                Actions = new List<Dialog>()
                                {
                                    new EmitEvent()
                                    {
                                        EventName = AdaptiveEvents.RecognizedIntent,
                                        EventValue = "=dialog.qnaResult.result"
                                    },
                                    new BreakLoop()
                                }
                            },

                            // Rule 4: QnA came back with no match => Pick LUIS result
                            new IfCondition()
                            {
                                Condition = "dialog.qnaResult.score <= 0.05",
                                Actions = new List<Dialog>()
                                {
                                    new EmitEvent()
                                    {
                                        EventName = AdaptiveEvents.RecognizedIntent,
                                        EventValue = "=dialog.luisResult.result"
                                    },
                                    new BreakLoop()
                                }
                            },

                            // None of the rules were true. So ask user to disambiguate. 
                            new TextInput()
                            {
                                Property = "turn.intentChoice",
                                Prompt = new ActivityTemplate("${chooseIntentResponseWithCard()}"),
                                // Adaptive card 'data' is automatically bound to entities or intent.
                                // You can include 'intent':'value' in your adaptive card's data to pick that up as an intent.
                                Value = "=@userChosenIntent",
                                AlwaysPrompt = true,
                                AllowInterruptions = false
                            },

                            // Decide which recognition result to use based on user response to disambiguation. 
                            new IfCondition()
                            {
                                Condition = "turn.intentChoice != 'none'",
                                Actions = new List<Dialog>()
                                {
                                    new EmitEvent()
                                    {
                                        EventName = AdaptiveEvents.RecognizedIntent,
                                        EventValue = "=dialog[turn.intentChoice].result"
                                    }
                                },
                                ElseActions = new List<Dialog>()
                                {
                                    new SendActivity()
                                    {
                                        Activity = new ActivityTemplate("Sure, no worries.")
                                    }
                                },
                            },
                        },
                    },
                },
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            this.AddDialog(this._rootDialog);

            // Add all child dialogS
            this.AddDialog(new AddToDoDialog(configuration));
            this.AddDialog(new DeleteToDoDialog(configuration));
            this.AddDialog(new ViewToDoDialog(configuration));
            this.AddDialog(new GetUserProfileDialog(configuration));

            // The initial child Dialog to run.
            this.InitialDialogId = nameof(RootDialog);
        }

        private async Task<DialogTurnResult> ResolveAndSendQnAAnswer(DialogContext dc, System.Object options)
        {
            var exp1 = Expression.Parse("@answer").TryEvaluate(dc.State).value;
            var resVal = await this._rootDialog.Generator.GenerateAsync(dc, exp1.ToString(), dc.State);
            await dc.Context.SendActivityAsync(ActivityFactory.FromObject(resVal));
            return await dc.EndDialogAsync(options);
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

        private static Recognizer CreateCrossTrainedRecognizer(IConfiguration configuration)
        {
            return new CrossTrainedRecognizerSet()
            {
                Recognizers = new List<Recognizer>()
                {
                    CreateLuisRecognizer(configuration),
                    CreateQnAMakerRecognizer(configuration)
                }
            };
        }

        private static Recognizer CreateQnAMakerRecognizer(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration["qna:TodoBotWithLuisAndQnA_en_us_qna"]) || string.IsNullOrEmpty(configuration["QnAHostName"]) || string.IsNullOrEmpty(configuration["QnAEndpointKey"]))
            {
                throw new Exception("NOTE: QnA Maker is not configured for RootDialog. Please follow instructions in README.md. To enable all capabilities, add 'qnamaker:qnamakerSampleBot_en_us_qna', 'qnamaker:LuisAPIKey' and 'qnamaker:endpointKey' to the appsettings.json file.");
            }

            return new QnAMakerRecognizer()
            {
                HostName = configuration["QnAHostName"],
                EndpointKey = configuration["QnAEndpointKey"],
                KnowledgeBaseId = configuration["qna:TodoBotWithLuisAndQnA_en_us_qna"],

                // property path that holds qna context
                Context = "dialog.qnaContext",

                // Property path where previous qna id is set. This is required to have multi-turn QnA working.
                QnAId = "turn.qnaIdFromPrompt",

                // Disable teletry logging
                LogPersonalInformation = false,

                // Enable to automatically including dialog name as meta data filter on calls to QnA Maker.
                IncludeDialogNameInMetadata = true,

                // Id needs to be QnA_<dialogName> for cross-trained recognizer to work.
                Id = $"QnA_{nameof(RootDialog)}"
            };
        }

        public static Recognizer CreateLuisRecognizer(IConfiguration Configuration)
        {
            if (string.IsNullOrEmpty(Configuration["luis:RootDialog_en_us_lu"]) || string.IsNullOrEmpty(Configuration["LuisAPIKey"]) || string.IsNullOrEmpty(Configuration["LuisAPIHostName"]))
            {
                throw new Exception("Your RootDialog LUIS application is not configured. Please see README.MD to set up a LUIS application.");
            }
            return new LuisAdaptiveRecognizer()
            {
                Endpoint = Configuration["LuisAPIHostName"],
                EndpointKey = Configuration["LuisAPIKey"],
                ApplicationId = Configuration["luis:RootDialog_en_us_lu"],

                // Id needs to be LUIS_<dialogName> for cross-trained recognizer to work.
                Id = $"LUIS_{nameof(RootDialog)}"
            };
        }
    }
}
