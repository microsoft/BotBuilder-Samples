// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.AI.Orchestrator;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class ViewToDoDialog : ComponentDialog
    {
        public ViewToDoDialog(IConfiguration configuration)
            : base(nameof(ViewToDoDialog))
        {
            string[] paths = { ".", "Dialogs", "ViewToDoDialog", "ViewToDoDialog.lg" };
            string fullPath = Path.Combine(paths);

            // Create instance of adaptive dialog. 
            var viewToDoDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(fullPath)),

                // Create and use a Orchestrator recognizer on the child
                // Each child adaptive dialog can have its own recognizer. 
                Recognizer = CreateOrchestratorRecognizer(configuration),
                Triggers = new List<OnCondition>()
                {
                    new OnBeginDialog() 
                    {
                        Actions = new List<Dialog>() 
                        {
                            // See if any list has any items.
                            new IfCondition()
                            {
                                Condition = "count(user.lists.todo) != 0 || count(user.lists.grocery) != 0 || count(user.lists.shopping) != 0",
                                Actions = new List<Dialog>()
                                {
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
                                            "contains(createArray('todo', 'shopping', 'grocery', 'all'), toLower(this.value))",
                                        },
                                        OutputFormat = "=toLower(this.value)",
                                        InvalidPrompt = new ActivityTemplate("${GetListType.Invalid()}"),
                                        MaxTurnCount = 2,
                                        DefaultValue = "todo",
                                        DefaultValueResponse = new ActivityTemplate("${GetListType.DefaultValueResponse()}")
                                    },
                                    new SendActivity("${ShowList()}")
                                },
                                ElseActions = new List<Dialog>()
                                {
                                    new SendActivity("${NoItemsInLists()}")
                                }
                            },
                        }
                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(viewToDoDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }

        private static Recognizer CreateOrchestratorRecognizer(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration["orchestrator:ModelPath"]) || string.IsNullOrEmpty(configuration["orchestrator:SnapShotPaths:ViewToDoDialog"]))
            {
                throw new Exception("Your RootDialog Orchestrator application is not configured. Please see README.MD to set it up.");
            }

            return new OrchestratorAdaptiveRecognizer()
            {
                ModelPath = configuration["orchestrator:ModelPath"],
                SnapshotPath = configuration["orchestrator:SnapShotPaths:ViewToDoDialog"],
            };
        }
    }
}
