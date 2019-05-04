using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions.Parser;

namespace Microsoft.BotBuilderSamples
{
    public class DeleteToDoDialog : ComponentDialog
    {
        public DeleteToDoDialog()
            : base(nameof(DeleteToDoDialog))
        {
            // Create instance of adaptive dialog. 
            var DeleteToDoDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Steps = new List<IDialog>()
                {
                    // Handle case where there are no items in todo list
                    new IfCondition()
                    {
                        // All conditions are expressed using the common expression language.
                        // See https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/common-expression-language to learn more
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
                    // prebuilt number entity that denotes the position of the todo item in the list .or.
                    // todoIdx machine learned entity that can detect things like first or last etc. 

                    // As a demonstration for this example, use a code step to understand entities returned by LUIS.
                    // You could have easily replaced the code step with these two steps
                    // new SaveEntity("@todoTitle[0]", "turn.todoTitle"),
                    // new SaveEntity("@todoTitle_patternAny[0]", "turn.todoTitle"),

                    new CodeStep(GetToDoTitleToDelete),
                    new IfCondition()
                    {
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
                    new EditArray()
                    {
                        ArrayProperty = "user.todos",
                        ItemProperty = "turn.todoTitle",
                        ChangeType = EditArray.ArrayChangeType.Clear
                    },
                    new SendActivity("[Delete-readBack]"),
                    new EndDialog()
                },
                Rules = new List<IRule>()
                {
                    // This event rule will catch outgoing bubbling up to the parent and will swallow anything that user says that is in the todo list. 
                    new EventRule()
                    {
                        // Consultation happens on every turn when using TextInput. This gives all parents a chance to take the user input before text input takes it.
                        Events = new List<string>() { AdaptiveEvents.ConsultDialog },
                        // The expression language is quite powerful with a bunch of pre-built utility functions.
                        // See https://github.com/Microsoft/BotBuilder-Samples/blob/master/experimental/common-expression-language/prebuilt-functions.md
                        Constraint = "contains(user.todos, turn.activity.text)",
                        Steps = new List<IDialog>()
                        {
                            // Take user input  as the title of the todo to delete if it exists
                            new SetProperty() {
                                Property = "turn.todoTitle",
                                Value = new ExpressionEngine().Parse("turn.activity.text")
                            }
                        }
                    }
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
            string[] todoTitle, todoTitle_patternAny;
            // By default, recognized intents from a recognizer are available under turn.intents scope. 
            // Recognized entities are available under turn.entities scope. 
            dc.State.TryGetValue("turn.entities.todoTitle", out todoTitle);
            dc.State.TryGetValue("turn.entities.todoTitle_patternAny", out todoTitle_patternAny);
            if (todoTitle != null && todoTitle.Length != 0)
            {
                if (Array.Exists(todoList, e => e == todoTitle[0])) {
                    todoTitleStr = todoTitle[0];
                }
            }
            else if (todoTitle_patternAny != null && todoTitle_patternAny.Length != 0)
            {
                if (Array.Exists(todoList, e => e == todoTitle_patternAny[0])) {
                    todoTitleStr = todoTitle_patternAny[0];
                }
            }
            if (todoTitleStr != null)
            {
                // Set the todo title in turn.todoTitle scope.
                dc.State.SetValue("turn.todoTitle", todoTitleStr);
            }
            return new DialogTurnResult(DialogTurnStatus.Complete, options);
        }
    }
}
