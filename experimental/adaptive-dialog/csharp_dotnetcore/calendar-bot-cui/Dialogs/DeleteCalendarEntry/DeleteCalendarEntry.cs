using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions.Parser;

namespace Microsoft.BotBuilderSamples
{
    public class DeleteCalendarEntry : ComponentDialog
    {
        public DeleteCalendarEntry()
            : base(nameof(DeleteCalendarEntry))
        {
            // Create instance of adaptive dialog. 
            var DeleteCalendarEntry = new AdaptiveDialog(nameof(AdaptiveDialog));
            DeleteCalendarEntry.Steps = new List<IDialog>()
                {
                    // Handle case where there are no items in todo list
                    new IfCondition()
                    {
                        Condition = new ExpressionEngine().Parse("user.Entries == null || count(user.Entries) <= 0"),
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[DeleteEmptyList]"),
                            new SendActivity("[Welcome-Actions]"),
                            new EndDialog()
                        }
                    },

                     new SaveEntity("@Subject[0]", "turn.entrySubject"),                    
                    // new CodeStep(GetToDoTitleToDelete),
                   
                    new IfCondition()
                    {
                        Condition = new ExpressionEngine().Parse("turn.entrySubject == null"),
                        Steps = new List<IDialog>()
                        {
                            // First show the current list of Todos
                            new BeginDialog(nameof(FindCalendarEntry)),
                            new TextInput()
                            {
                                Property = "turn.entrySubject",
                                Prompt = new ActivityTemplate("[GetEntryTitleToDelete]"),
                            }
                        }
                    },
                    new IfCondition()
                    {
                        Condition = new ExpressionEngine().Parse("contains(user.todos, turn.entrySubject) == false"),
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[EntryNotFound]"),
                            new DeleteProperty()
                            {
                                Property = "turn.entrySubject"
                            },
                            new RepeatDialog()
                        }
                    },
                    new EditArray()
                    {
                        ArrayProperty = "user.entries",
                        ItemProperty = "turn.entrySubject",
                        ChangeType = EditArray.ArrayChangeType.Clear
                    },
                    new SendActivity("[DeleteReadBack]"),
                    new EndDialog()
                };
            DeleteCalendarEntry.Rules = new List<IRule>()
                {
                    new IntentRule("CheckMessages")
                    {
                        Steps = new List<IDialog>() { new BeginDialog(nameof(DeleteCalendarEntry), null) },
                        Constraint = "turn.dialogEvent.value.intents.CheckMessages.score > 0.4"
                    }
                    //    // This event rule will catch outgoing bubbling up to the parent and will swallow anything that user says that is in the todo list. 
                    //    new EventRule()
                    //    {
                    //        Events = new List<string>() { AdaptiveEvents.ConsultDialog },
                    //        Constraint = "contains(user.todos, turn.activity.text)",
                    //        Steps = new List<IDialog>()
                    //        {
                    //            // Take user input  as the title of the todo to delete if it exists
                    //            new SetProperty() {
                    //                Property = "turn.entrySubject",
                    //                Value = new ExpressionEngine().Parse("turn.activity.text")
                    //            }
                    //        }
                    //    }
                };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(DeleteCalendarEntry);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }

        /* out source code to get the title
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
                if (Array.Exists(todoList, e => e == todoTitle[0]))
                {
                    todoTitleStr = todoTitle[0];
                }
            }
            else if (todoTitle_patternAny != null && todoTitle_patternAny.Length != 0)
            {
                if (Array.Exists(todoList, e => e == todoTitle_patternAny[0]))
                {
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
        */
    }
}
