using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions.Parser;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class AddToDoDialog : ComponentDialog
    {
        private static IConfiguration Configuration;

        public AddToDoDialog(IConfiguration configuration)
            : base(nameof(AddToDoDialog))
        {

            Configuration = configuration;
            // Create instance of adaptive dialog. 
            var AddToDoDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                // Create and use a regex recognizer on the child
                Recognizer = CreateRecognizer(),
                Steps = new List<IDialog>()
                {
                    // Take todo title if we already have it from root dialog's LUIS model.
                    // This is the title entity defined in ../RootDialog/RootDialog.lu.
                    // There is one LUIS application for this bot. So any entity captured by the rootDialog
                    // will be automatically available to child dialog.
                    // @EntityName is a short-hand for turn.entities.<EntityName>. Other useful short-hands are 
                    //     #IntentName is a short-hand for turn.intents.<IntentName>
                    //     $PropertyName is a short-hand for dialog.results.<PropertyName>
                    new SaveEntity() {
                        Property = "turn.todoTitle",
                        Entity = "@todoTitle[0]"
                    },
                    new SaveEntity() {
                        Property = "turn.todoTitle",
                        Entity = "@todoTitle_patternAny[0]"
                    },
                    // TextInput by default will skip the prompt if the property has value.
                    new TextInput()
                    {
                        Property = "turn.todoTitle",
                        Prompt = new ActivityTemplate("[Get-ToDo-Title]")
                    },
                    // Add the new todo title to the list of todos. Keep the list of todos in the user scope.
                    new EditArray()
                    {
                        ItemProperty = "turn.todoTitle",
                        ArrayProperty = "user.todos",
                        ChangeType = EditArray.ArrayChangeType.Push
                    },
                    new SendActivity("[Add-ToDo-ReadBack]")
                    // All child dialogs will automatically end if there are no additional steps to execute. 
                    // If you wish for a child dialog to not end automatically, you can set 
                    // AutoEndDialog property on the Adaptive Dialog to 'false'
                },
                Rules = new List<IRule>()
                {
                    // Handle local intents.
                    // All local intents are defined in AddToDoDialog.lu file.
                    new IntentRule("GetTodoTitle")
                    {
                        Steps = new List<IDialog>()
                        {
                            new SaveEntity("@todoTitle_patternAny[0]", "turn.todoTitle"),
                            new SaveEntity("todoTitle[0]", "turn.todoTitle")
                        }
                    },
                    new IntentRule("Help")
                    {
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[Help-Add-ToDo]")
                        }
                    },
                    new IntentRule("Cancel")
                    {
                        Steps = new List<IDialog>()
                        {
                            new ConfirmInput()
                            {
                                Property = "turn.addTodo.cancelConfirmation",
                                Prompt = new ActivityTemplate("[Confirm-cancellation]")
                            },
                            new IfCondition()
                            {
                                Condition = new ExpressionEngine().Parse("turn.addTodo.cancelConfirmation == true"),
                                Steps = new List<IDialog>()
                                {
                                    new SendActivity("[Cancel-add-todo]"),
                                    new EndDialog()
                                },
                                ElseSteps = new List<IDialog>()
                                {
                                    new SendActivity("[Help-prefix], let's get right back to adding a todo.")
                                }
                                // We do not need to specify an else block here since if user said no,
                                // the control flow will automatically return to the last active step (if any)
                            }
                        }
                    },
                    // Since we are using a regex recognizer, anything except for help or cancel will come back as none intent.
                    // If so, just accept user's response as the title of the todo and move forward.
                    new IntentRule("None")
                    {
                        Steps = new List<IDialog>()
                        {
                            // Set what user said as the todo title.
                            new SetProperty() {
                                Property = "turn.todoTitle",
                                Value = new ExpressionEngine().Parse("turn.activity.text")
                            }
                        }
                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(AddToDoDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }

        private static IRecognizer CreateRecognizer()
        {
            if (string.IsNullOrEmpty(Configuration["AddToDoDialog_en-us_lu:Luis-host-name"]) || string.IsNullOrEmpty(Configuration["AddToDoDialog_en-us_lu:Luis-endpoint-key"]) || string.IsNullOrEmpty(Configuration["AddToDoDialog_en-us_lu:Luis-app-id"]))
            {
                throw new Exception("Your LUIS application is not configured. Please see README.MD to set up a LUIS application.");
            }
            return new LuisRecognizer(new LuisApplication()
            {
                Endpoint = Configuration["AddToDoDialog_en-us_lu:Luis-host-name"],
                EndpointKey = Configuration["AddToDoDialog_en-us_lu:Luis-endpoint-key"],
                ApplicationId = Configuration["AddToDoDialog_en-us_lu:Luis-app-id"]
            });
        }
    }
}
