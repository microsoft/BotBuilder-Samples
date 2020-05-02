using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
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
                Generator = new TemplateEngineLanguageGenerator(Templates.ParseFile(fullPath)),
                // Create and use a regex recognizer on the child
                // Each child adaptive dialog can have its own recognizer. 
                // This sample demonstrates use of a regex recognizer in a child dialog. 
                Recognizer = CreateRegeExRecognizer(),
                Triggers = new List<OnCondition>()
                {
                    new OnBeginDialog() 
                    {
                        Actions = new List<Dialog>()
                        {
                            // Take todo title if we already have it from root dialog's LUIS model.
                            // This is the title entity defined in ../RootDialog/RootDialog.lu.
                            // There is one LUIS application for this bot. So any entity captured by the rootDialog
                            // will be automatically available to child dialog.
                            // @EntityName is a short-hand for turn.entities.<EntityName>. Other useful short-hands are 
                            //     #IntentName is a short-hand for turn.intents.<IntentName>
                            //     $PropertyName is a short-hand for dialog.<PropertyName>
                            new SetProperty() {
                                Property = "turn.todoTitle",
                                Value = "=@todoTitle"
                            },
                            // TextInput by default will skip the prompt if the property has value.
                            new TextInput()
                            {
                                Property = "turn.todoTitle",
                                Prompt = new ActivityTemplate("${GetToDoTitle()}")
                            },
                            // Add the new todo title to the list of todos. Keep the list of todos in the user scope.
                            new EditArray()
                            {
                                ItemsProperty = "user.todos",
                                ChangeType = EditArray.ArrayChangeType.Push,
                                Value = "=turn.todoTitle"
                            },
                            new SendActivity("${AddToDoReadBack()}")
                            // All child dialogs will automatically end if there are no additional steps to execute. 
                            // If you wish for a child dialog to not end automatically, you can set 
                            // AutoEndDialog property on the Adaptive Dialog to 'false'
                        }
                    },
                    // Since we are using regex recognizer, we will take any user input that does not match 
                    // help/ cancel/ confirmation intent to be the actual title of the todo item.
                    new OnIntent("None")
                    {
                        Actions = new List<Dialog>() 
                        {
                            new SetProperty() 
                            {
                                Property = "turn.todoTitle",
                                Value = "=turn.activity.text"
                            }
                        }
                    },
                    // Handle local help
                    new OnIntent("Help")
                    {
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("${HelpAddToDo()}")
                        }
                    },
                    new OnIntent("Cancel")
                    {
                        Actions = new List<Dialog>()
                        {
                            new ConfirmInput()
                            {
                                Property = "turn.addTodo.cancelConfirmation",
                                Prompt = new ActivityTemplate("${ConfirmCancellation()}"),
                                // Allow interruptions is an expression. So you can write any expression to determine if an interruption should be allowed.
                                // In this case, we will disallow interruptions since this is a cancellation confirmation. 
                                AllowInterruptions = false,
                                // Controls the number of times user is prompted for this input.
                                MaxTurnCount = 1,
                                // Default value to use if we have hit the MaxTurnCount
                                DefaultValue = "=false",
                                // You can refer to properties of this input via %propertyName notation. 
                                // The default response is sent if we have prompted the user for MaxTurnCount number of times
                                // and if a default value is assumed for the property.
                                DefaultValueResponse = new ActivityTemplate("Sorry, I do not recognize '${this.value}'. I'm going with '${%DefaultValue}' for now to be safe.")
                            },
                            new IfCondition()
                            {
                                // All conditions are expressed using the common expression language.
                                // See https://github.com/Microsoft/BotBuilder-Samples/tree/master/experimental/common-expression-language to learn more
                                Condition = "turn.addTodo.cancelConfirmation == true",
                                Actions = new List<Dialog>()
                                {
                                    new SendActivity("${CancelAddTodo()}"),
                                    new EndDialog()
                                },
                                ElseActions = new List<Dialog>()
                                {
                                    new SendActivity("${HelpPrefix()}, let's get right back to adding a todo.")
                                }
                                // We do not need to specify an else block here since if user said no,
                                // the control flow will automatically return to the last active step (if any)
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

        private static Recognizer CreateRegeExRecognizer()
        {
            return new RegexRecognizer()
            {
                Intents = new List<IntentPattern>
                {
                    new IntentPattern() { 
                        Intent = "Help", 
                        Pattern = "(?i)help" 
                    },
                    new IntentPattern() {
                        Intent = "Cancel", 
                        Pattern = "(?i)cancel|never mind" 
                    }
                },
                // Regex recognizer now supports pre-built entity types as well. 
                // See here for a list of supported entity recognizers - 
                // https://github.com/microsoft/botbuilder-dotnet/tree/master/libraries/Microsoft.Bot.Builder.Dialogs.Adaptive/Recognizers/EntityRecognizers
                // Entities = new List<EntityRecognizer>() {
                //     new DateTimeEntityRecognizer(),
                //     new NumberEntityRecognizer(),
                //     new AgeEntityRecognizer()
                // }
            };
        }
    }
}
