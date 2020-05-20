// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using Microsoft.Bot.Builder.AI.Luis;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.AI.QnA.Recognizers;
using AdaptiveExpressions;
using Microsoft.Bot.Builder;

namespace Microsoft.BotBuilderSamples
{
    public class DeleteToDoDialog : ComponentDialog
    {
        private AdaptiveDialog _deleteToDoDialog;
        public DeleteToDoDialog(IConfiguration configuration)
            : base(nameof(DeleteToDoDialog))
        {
            string[] paths = { ".", "Dialogs", "DeleteToDoDialog", "DeleteToDoDialog.lg" };
            string fullPath = Path.Combine(paths);
            // Create instance of adaptive dialog. 
            this._deleteToDoDialog = new AdaptiveDialog(nameof(DeleteToDoDialog))
            {
                Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(fullPath)),
                Recognizer = CreateCrossTrainedRecognizer(configuration),
                Triggers = new List<OnCondition>()
                {
                    new OnBeginDialog() 
                    {
                        Actions = new List<Dialog>() 
                        {
                            // Handle case where there are no items in todo list
                            new IfCondition()
                            {
                                // All conditions are expressed using adaptive expressions.
                                // See https://aka.ms/adaptive-expressions to learn more
                                Condition = "count(user.lists.todo) == 0 && count(user.lists.grocery) == 0 && count(user.lists.shopping) == 0",
                                Actions = new List<Dialog>()
                                {
                                    new SendActivity("${DeleteEmptyList()}"),
                                    new EndDialog()
                                }
                            },

                            // User could have specified the item and/ or list type to delete.
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

                            // Ask for list type first.
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

                            new IfCondition()
                            {
                                Condition = "count(user.lists[dialog.listType]) == 0",
                                Actions = new List<Dialog>()
                                {
                                    new SendActivity("${NoItemsInList()}"),
                                    new EndDialog()
                                }
                            },

                            // Ask for title to delete
                            new ChoiceInput()
                            {
                                Choices = "user.lists[dialog.listType]",
                                Property = "dialog.itemTitle",
                                OutputFormat = ChoiceOutputFormat.Value,
                                Style = ListStyle.List,
                                Prompt = new ActivityTemplate("${GetItemTitleToDelete()}")
                            },

                            // remove item
                            new EditArray()
                            {
                                ItemsProperty = "user.lists[dialog.listType]",
                                Value = "=dialog.itemTitle",
                                ChangeType = EditArray.ArrayChangeType.Remove
                            },

                            new SendActivity("${DeleteConfirmationReadBack()}")
                        }
                    },
                    // Shows how to use dialog event to capture intent recognition event for more than one intent.
                    // Alternate to this would be to add two separate OnIntent events.
                    // This ensures we set any entities recognized by these two intents.
                    new OnDialogEvent()
                    {
                        Event = AdaptiveEvents.RecognizedIntent,
                        Condition = "#GetTitleToDelete || #GetListType",
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
                            new SendActivity("${@Answer}")
                        }
                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(this._deleteToDoDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(DeleteToDoDialog);
        }

        private async Task<DialogTurnResult> ResolveAndSendQnAAnswer(DialogContext dc, System.Object options)
        {
            var exp1 = Expression.Parse("@answer").TryEvaluate(dc.State).value;
            var resVal = await this._deleteToDoDialog.Generator.Generate(dc, exp1.ToString(), dc.State);
            await dc.Context.SendActivityAsync(ActivityFactory.FromObject(resVal));
            return await dc.EndDialogAsync(options);
        }

        private async Task<DialogTurnResult> GetToDoTitleToDelete(DialogContext dc, System.Object options)
        {
            // Demonstrates using a custom code step to extract entities and set them in state.
            var todoList = dc.State.GetValue<string[]>("user.todos");
            string todoTitleStr = null;
            string[] todoTitle;
            // By default, recognized intents from a recognizer are available under turn.intents scope. 
            // Recognized entities are available under turn.entities scope. 
            dc.State.TryGetValue("turn.entities.todoTitle", out todoTitle);
            if (todoTitle != null && todoTitle.Length != 0)
            {
                if (Array.Exists(todoList, e => e == todoTitle[0])) {
                    todoTitleStr = todoTitle[0];
                }
            }
            if (todoTitleStr != null)
            {
                // Set the todo title in turn.todoTitle scope.
                dc.State.SetValue("turn.todoTitle", todoTitleStr);
            }
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
                Id = $"QnA_{nameof(DeleteToDoDialog)}"
            };
        }
        private static Recognizer CreateLuisRecognizer(IConfiguration Configuration)
        {
            if (string.IsNullOrEmpty(Configuration["luis:DeleteToDoDialog_en_us_lu"]) || string.IsNullOrEmpty(Configuration["LuisAPIKey"]) || string.IsNullOrEmpty(Configuration["LuisAPIHostName"]))
            {
                throw new Exception("Your AddToDoDialog's LUIS application is not configured for AddToDoDialog. Please see README.MD to set up a LUIS application.");
            }
            return new LuisAdaptiveRecognizer()
            {
                Endpoint = Configuration["LuisAPIHostName"],
                EndpointKey = Configuration["LuisAPIKey"],
                ApplicationId = Configuration["luis:DeleteToDoDialog_en_us_lu"],

                // Id needs to be LUIS_<dialogName> for cross-trained recognizer to work.
                Id = $"LUIS_{nameof(DeleteToDoDialog)}"
            };
        }
    }
}
