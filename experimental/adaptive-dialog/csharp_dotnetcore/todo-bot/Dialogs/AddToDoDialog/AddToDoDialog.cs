using System.Collections.Generic;
using Microsoft.Bot.Builder;
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

        public AddToDoDialog()
            : base(nameof(AddToDoDialog))
        {

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
                    new SaveEntity("turn.addTodo.title", "@todoTitle[0]"),
                    // TextInput by default will skip the prompt if the property has value.
                    new TextInput()
                    {
                        Property = "turn.addTodo.title",
                        Prompt = new ActivityTemplate("[Get-ToDo-Title]")
                    },
                    // Add the new todo title to the list of todos
                    new EditArray()
                    {
                        ItemProperty = "user.todos",
                        ArrayProperty = "turn.addTodo.title",
                        ChangeType = EditArray.ArrayChangeType.Push
                    },
                    new SendActivity("[Add-ToDo-ReadBack]"),
                    new EndDialog()
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
                                Condition = new ExpressionEngine().Parse("turn.addTodo.cancelConfirmation == true"),
                                Steps = new List<IDialog>()
                                {
                                    new EndDialog()
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
                            new SendActivity("Entities: {turn.entities}"),
                            new SendActivity("Activity: {turn.activity.text}"),
                            new SetProperty() {
                                Property = "turn.addTodo.title",
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
