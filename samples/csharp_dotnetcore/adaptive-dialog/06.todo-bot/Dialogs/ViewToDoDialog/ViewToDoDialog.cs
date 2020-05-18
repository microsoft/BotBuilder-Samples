// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using AdaptiveExpressions.Properties;

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
            var ViewToDoDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(fullPath)),
                // Create and use a LUIS recognizer on the child
                // Each child adaptive dialog can have its own recognizer. 
                Recognizer = CreateLuisRecognizer(configuration),
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
            AddDialog(ViewToDoDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }

        private static Recognizer CreateLuisRecognizer(IConfiguration Configuration)
        {
            if (string.IsNullOrEmpty(Configuration["luis:ViewToDoDialog_en_us_lu"]) || string.IsNullOrEmpty(Configuration["LuisAPIKey"]) || string.IsNullOrEmpty(Configuration["LuisAPIHostName"]))
            {
                throw new Exception("Your AddToDoDialog's LUIS application is not configured for ViewToDoDialog. Please see README.MD to set up a LUIS application.");
            }
            return new LuisAdaptiveRecognizer()
            {
                Endpoint = Configuration["LuisAPIHostName"],
                EndpointKey = Configuration["LuisAPIKey"],
                ApplicationId = Configuration["luis:ViewToDoDialog_en_us_lu"]
            };
        }
    }
}
