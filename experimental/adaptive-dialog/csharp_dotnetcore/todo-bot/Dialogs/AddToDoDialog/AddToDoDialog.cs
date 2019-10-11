using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.LanguageGeneration;

namespace Microsoft.BotBuilderSamples
{
    public class AddToDoDialog : ComponentDialog
    {
        public AddToDoDialog()
            : base(nameof(AddToDoDialog))
        {
            string[] paths = { ".", "Dialogs", "AddToDoDialog", "AddToDoDialog.lg" };
            string fullPath = Path.Combine(paths);
            // Create instance of adaptive dialog. 
            var AddToDoDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new TemplateEngineLanguageGenerator(new TemplateEngine().AddFile(fullPath)),
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
                    new SetProperty() {
                        Property = "turn.todoTitle",
                        Value = "turn.recognized.entities.todoTitle[0]"
                    },
                    new SetProperty() {
                        Property = "turn.todoTitle",
                        Value = "turn.recognized.entities.todoTitle_patternAny[0]"
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
                        Value = "turn.todoTitle",
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
                    // Handle local help
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
                                // All conditions are expressed using the common expression language.
                                // See https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/common-expression-language to learn more
                                Condition = "turn.addTodo.cancelConfirmation == true",
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
                                Value = "turn.activity.text"
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
            return new RegexRecognizer()
            {
                Intents = new Dictionary<string, string>()
                {
                    { "Help", "(?i)help" },
                    { "Cancel", "(?i)cancel|never mind" }
                }
            };
        }
    }
}
