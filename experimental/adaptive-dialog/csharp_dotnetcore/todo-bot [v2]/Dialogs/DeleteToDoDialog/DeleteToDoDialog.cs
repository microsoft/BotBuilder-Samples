using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions.Parser;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples
{
    public class DeleteToDoDialog : ComponentDialog
    {
        private static IConfiguration Configuration;

        public DeleteToDoDialog(IConfiguration configuration)
            : base(nameof(DeleteToDoDialog))
        {
            Configuration = configuration;
            // Create instance of adaptive dialog. 
            var DeleteToDoDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Steps = new List<IDialog>()
                {
                    // Handle case where there are no items in todo list
                    new IfCondition()
                    {
                        Condition = new ExpressionEngine().Parse("user.todos == null || count(user.todos) <= 0"),
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[Delete-Empty-List]"),
                            new SendActivity("[Welcome-Actions]"),
                            new EndDialog()
                        }
                    },
                    // User could have already specified the todo to delete via 
                    // todoTitle as simple machine learned LUIS entity or
                    // todoTitle_patternAny as pattern.any LUIS entity .or.
                    // ordinal entity that denotes the position of the todo item in the list  

                    // As a demonstration for this sample, use a code step to understand entities returned by LUIS.
                    // You could have easily replaced the code step with these two steps
                    // new SaveEntity("@todoTitle[0]", "turn.todoTitle"),
                    // new SaveEntity("@todoTitle_patternAny[0]", "turn.todoTitle"),
                    // new SaveEntity("user.todos[@ordinal[0]]", "turn.todoTitle"),
                    new CodeStep(GetToDoTitleToDelete),
                    new IfCondition()
                    {
                        // Ask for title if we do not have yet it.
                        Condition = new ExpressionEngine().Parse("turn.todoTitle == null"),
                        Steps = new List<IDialog>()
                        {
                            // First show the current list of Todos
                            new BeginDialog(nameof(ViewToDoDialog)),
                            new TextInput()
                            {
                                Property = "turn.todoTitle",
                                Prompt = new ActivityTemplate("[Get-ToDo-Title-To-Delete]"),
                            }
                        }
                    },
                    new IfCondition()
                    {
                        // If the title we got does not exist in the todo list, repeat dialog.
                        Condition = new ExpressionEngine().Parse("contains(user.todos, turn.todoTitle) == false"),
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[Todo-not-found]"),
                            new DeleteProperty()
                            {
                                Property = "turn.todoTitle"
                            },
                            new RepeatDialog()
                        }
                    },
                    // Delete the todo item from the list.
                    new EditArray()
                    {
                        ArrayProperty = "user.todos",
                        ItemProperty = "turn.todoTitle",
                        ChangeType = EditArray.ArrayChangeType.Remove
                    },
                    // Confirm deletion.
                    new SendActivity("[Delete-readBack]"),
                    // Readback available Todos list
                    new BeginDialog(nameof(ViewToDoDialog)),
                    new EndDialog()
                },
                Rules = new List<IRule>()
                {
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
                    },
                    new IntentRule("GetItemToDelete")
                    {
                        Steps = new List<IDialog>()
                        {
                            // call the code step to accept all provided values
                            new CodeStep(GetToDoTitleToDelete)
                        }
                    }
                    // Note: Help and Cancel intents are deliberately not handled here to demonstrate bubbling up to the RootDialog.
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(DeleteToDoDialog);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }

        private async Task<DialogTurnResult> GetToDoTitleToDelete(DialogContext dc, System.Object options)
        {
            // Demonstrates using a custom code step to extract entities and set them in state.
            var todoList = dc.State.GetValue<string[]>("user.todos");
            string todoTitleStr = null;
            string[] todoTitle, todoTitle_patternAny, ordinal;
            // By default, recognized intents from a recognizer are available under turn.intents scope. 
            // Recognized entities are available under turn.entities scope. 
            dc.State.TryGetValue("turn.entities.todoTitle", out todoTitle);
            dc.State.TryGetValue("turn.entities.todoTitle_patternAny", out todoTitle_patternAny);
            dc.State.TryGetValue("turn.entities.ordinal", out ordinal);
            if (todoTitle != null && todoTitle.Length != 0)
            {
                if (Array.Exists(todoList, e => e == todoTitle[0])) {
                    todoTitleStr = todoTitle[0];
                }
            }
            if (todoTitle_patternAny != null && todoTitle_patternAny.Length != 0)
            {
                if (Array.Exists(todoList, e => e == todoTitle_patternAny[0])) {
                    todoTitleStr = todoTitle_patternAny[0];
                }
            }
            if (ordinal != null && ordinal.Length != 0)
            {
                try
                {
                    todoTitleStr = todoList[Convert.ToInt32(ordinal[0]) - 1];
                }
                catch
                {
                    todoTitleStr = null;
                }
            }
            if (todoTitleStr != null)
            {
                // Set the todo title in turn.todoTitle scope.
                dc.State.SetValue("turn.todoTitle", todoTitleStr);
            }
            return new DialogTurnResult(DialogTurnStatus.Complete, options);
        }

        public static IRecognizer CreateRecognizer()
        {
            if (string.IsNullOrEmpty(Configuration["DeleteToDoDialog_en-us_lu:Luis-host-name"]) || string.IsNullOrEmpty(Configuration["DeleteToDoDialog_en-us_lu:Luis-endpoint-key"]) || string.IsNullOrEmpty(Configuration["DeleteToDoDialog_en-us_lu:Luis-app-id"]))
            {
                throw new Exception("Your LUIS application is not configured. Please see README.MD to set up a LUIS application.");
            }
            return new LuisRecognizer(new LuisApplication()
            {
                Endpoint = Configuration["DeleteToDoDialog_en-us_lu:Luis-host-name"],
                EndpointKey = Configuration["DeleteToDoDialog_en-us_lu:Luis-endpoint-key"],
                ApplicationId = Configuration["DeleteToDoDialog_en-us_lu:Luis-app-id"]
            });
        }
    }
}
