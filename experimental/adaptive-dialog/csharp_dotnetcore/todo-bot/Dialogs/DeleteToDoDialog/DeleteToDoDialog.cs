using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.LanguageGeneration;

namespace Microsoft.BotBuilderSamples
{
    public class DeleteToDoDialog : ComponentDialog
    {
        public DeleteToDoDialog()
            : base(nameof(DeleteToDoDialog))
        {
            string[] paths = { ".", "Dialogs", "DeleteToDoDialog", "DeleteToDoDialog.lg" };
            string fullPath = Path.Combine(paths);
            // Create instance of adaptive dialog. 
            var DeleteToDoDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new TemplateEngineLanguageGenerator(new TemplateEngine().AddFile(fullPath)),
                Steps = new List<IDialog>()
                {
                    // Handle case where there are no items in todo list
                    new IfCondition()
                    {
                        // All conditions are expressed using the common expression language.
                        // See https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/common-expression-language to learn more
                        Condition = "user.todos == null || count(user.todos) <= 0",
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
                        Condition = "turn.todoTitle == null",
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
                        Condition = "contains(user.todos, turn.todoTitle) == false",
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
                        Value = "turn.todoTitle",
                        ChangeType = EditArray.ArrayChangeType.Clear
                    },
                    new SendActivity("[Delete-readBack]"),
                    new EndDialog()
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
