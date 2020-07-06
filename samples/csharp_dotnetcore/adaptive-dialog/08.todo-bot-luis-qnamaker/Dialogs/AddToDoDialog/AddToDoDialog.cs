// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using System;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.AI.QnA.Recognizers;
using System.Threading.Tasks;
using AdaptiveExpressions;
using Microsoft.Bot.Builder;

namespace Microsoft.BotBuilderSamples
{
    public class AddToDoDialog : ComponentDialog
    {
        private static IConfiguration Configuration;
        private AdaptiveDialog _addToDoDialog;
        public AddToDoDialog(IConfiguration configuration)
            : base(nameof(AddToDoDialog))
        {
            Configuration = configuration;
            string[] paths = { ".", "Dialogs", "AddToDoDialog", "AddToDoDialog.lg" };
            string fullPath = Path.Combine(paths);
            // Create instance of adaptive dialog. 
            this._addToDoDialog = new AdaptiveDialog(nameof(AddToDoDialog))
            {
                Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(fullPath)),
                // Create and use a LUIS recognizer on the child
                // Each child adaptive dialog can have its own recognizer. 
                Recognizer = CreateCrossTrainedRecognizer(configuration),
                Triggers = new List<OnCondition>()
                {
                    new OnBeginDialog() 
                    {
                        Actions = new List<Dialog>()
                        {
                            // Take todo title if we already have it from root dialog's LUIS model.
                            // There is one LUIS application for this bot. So any entity captured by the rootDialog
                            // will be automatically available to child dialog.
                            // @EntityName is a short-hand for turn.recognized.entities.<EntityName>. Other useful short-hands are 
                            //     #IntentName is a short-hand for turn.intents.<IntentName>
                            //     $PropertyName is a short-hand for dialog.<PropertyName>
                            new SetProperties()
                            {
                                Assignments = new List<PropertyAssignment>()
                                {
                                    new PropertyAssignment()
                                    {
                                        Property = "dialog.itemTitle",
                                        Value = "=@itemTitle"
                                    },
                                    new PropertyAssignment()
                                    {
                                        Property = "dialog.listType",
                                        Value = "=@listType"
                                    }
                                }
                            },
                            // TextInput by default will skip the prompt if the property has value.
                            new TextInput()
                            {
                                Property = "dialog.itemTitle",
                                Prompt = new ActivityTemplate("${GetItemTitle()}"),
                                // This entity is coming from the local AddToDoDialog's own LUIS recognizer.
                                // This dialog's .lu file is under ./AddToDoDialog.lu
                                Value = "=@itemTitle",
                                // Allow interruption if we do not have an item title and have a super high confidence classification of an intent.
                                AllowInterruptions = "!@itemTitle && turn.recognized.score >= 0.7"
                            },
                            // Get list type
                            new TextInput()
                            {
                                Property = "dialog.listType",
                                Prompt = new ActivityTemplate("${GetListType()}"),
                                Value = "=@listType",
                                AllowInterruptions = "!@listType && turn.recognized.score >= 0.7",
                                Validations = new List<BoolExpression>()
                                {
                                    // Verify using expressions that the value is one of todo or shopping or grocery
                                    "contains(createArray('todo', 'shopping', 'grocery'), toLower(this.value))",
                                },
                                OutputFormat = "=toLower(this.value)",
                                InvalidPrompt = new ActivityTemplate("${GetListType.Invalid()}"),
                                MaxTurnCount = 2,
                                DefaultValue = "todo",
                                DefaultValueResponse = new ActivityTemplate("${GetListType.DefaultValueResponse()}")
                            },
                            // Add the new todo title to the list of todos. Keep the list of todos in the user scope.
                            new EditArray()
                            {
                                ItemsProperty = "user.lists[dialog.listType]",
                                ChangeType = EditArray.ArrayChangeType.Push,
                                Value = "=dialog.itemTitle"
                            },
                            new SendActivity("${AddItemReadBack()}")
                            // All child dialogs will automatically end if there are no additional steps to execute. 
                            // If you wish for a child dialog to not end automatically, you can set 
                            // AutoEndDialog property on the Adaptive Dialog to 'false'
                        }
                    },
                    // Although root dialog can handle this, this will match loacally because this dialog's .lu has local definition for this intent. 
                    new OnIntent("Help")
                    {
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("${HelpAddItem()}")
                        }
                    },
                    // Shows how to use dialog event to capture intent recognition event for more than one intent.
                    // Alternate to this would be to add two separate OnIntent events.
                    // This ensures we set any entities recognized by these two intents.
                    new OnDialogEvent()
                    {
                        Event = AdaptiveEvents.RecognizedIntent,
                        Condition = "#GetItemTitle || #GetListType",
                        Actions = new List<Dialog>()
                        {
                            new SetProperties()
                            {
                                Assignments = new List<PropertyAssignment>()
                                {
                                    new PropertyAssignment()
                                    {
                                        Property = "dialog.itemTitle",
                                        Value = "=@itemTitle"
                                    },
                                    new PropertyAssignment()
                                    {
                                        Property = "dialog.listType",
                                        Value = "=@listType"
                                    }
                                }
                            }
                        }
                    },
                    // Help and chitchat is handled by qna
                    new OnQnAMatch
                    {
                        Actions = new List<Dialog>()
                        {
                            new CodeAction(ResolveAndSendQnAAnswer)
                        }
                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(this._addToDoDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AddToDoDialog);
        }

        private async Task<DialogTurnResult> ResolveAndSendQnAAnswer(DialogContext dc, System.Object options)
        {
            var exp1 = Expression.Parse("@answer").TryEvaluate(dc.State).value;
            var resVal = await this._addToDoDialog.Generator.GenerateAsync(dc, exp1.ToString(), dc.State);
            await dc.Context.SendActivityAsync(ActivityFactory.FromObject(resVal));
            return await dc.EndDialogAsync(options);
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
                Id = $"QnA_{nameof(AddToDoDialog)}"
            };
        }

        private static Recognizer CreateLuisRecognizer(IConfiguration Configuration)
        {
            if (string.IsNullOrEmpty(Configuration["luis:AddToDoDialog_en_us_lu"]) || string.IsNullOrEmpty(Configuration["LuisAPIKey"]) || string.IsNullOrEmpty(Configuration["LuisAPIHostName"]))
            {
                throw new Exception("Your AddToDoDialog's LUIS application is not configured for AddToDoDialog. Please see README.MD to set up a LUIS application.");
            }
            return new LuisAdaptiveRecognizer()
            {
                Endpoint = Configuration["LuisAPIHostName"],
                EndpointKey = Configuration["LuisAPIKey"],
                ApplicationId = Configuration["luis:AddToDoDialog_en_us_lu"],

                // Id needs to be LUIS_<dialogName> for cross-trained recognizer to work.
                Id = $"LUIS_{nameof(AddToDoDialog)}"
            };
        }
    }
}
