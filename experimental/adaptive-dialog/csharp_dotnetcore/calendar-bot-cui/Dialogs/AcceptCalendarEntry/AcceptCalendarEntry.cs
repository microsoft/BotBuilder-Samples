using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions;
using Microsoft.Bot.Builder.Expressions.Parser;
using Microsoft.Bot.Builder.LanguageGeneration;

namespace Microsoft.BotBuilderSamples
{
    public class AcceptCalendarEntry : ComponentDialog
    {
        public AcceptCalendarEntry()
            : base(nameof(AcceptCalendarEntry))
        {
            // Create instance of adaptive dialog. 
            var acceptCalendarEntry = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new ResourceMultiLanguageGenerator("AcceptCalendarEntry.lg"),
                Steps = new List<IDialog>()
                {
                    // Handle case where there are no items in todo list
                    new IfCondition()
                    {
                        Condition = new ExpressionEngine().Parse("user.Entries == null || count(user.Entries) <= 0"),
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[AccpetEmptyList]"),
                            new SendActivity("[Welcome-Actions]"),
                            new EndDialog()
                        }
                    },

                    // new SaveEntity("@Subject[0]", "dialog.deleteCalendarEntry_entrySubject"),                    
                    // new CodeStep(GetToDoTitleToDelete),
                   
                    //new IfCondition()
                    //{
                    //    Steps = new List<IDialog>()
                    //    {
                    new BeginDialog(nameof(FindCalendarEntry)),
                    new TextInput() //TODO BUGS exist here wil jump and without prompting users
                    {
                        Property = "dialog.acceptCalendarEntry_entrySubject",
                        Prompt = new ActivityTemplate("[GetEntryTitleToDelete]"),
                    },
                    //    }
                    //},

                    new InitProperty()
                    {
                        Property = "dialog.afterAccepted",
                        Type = "array"
                    },

                    new Foreach()
                    {
                        ListProperty = new ExpressionEngine().Parse("user.Entries"),
                        Steps = new List<IDialog>()
                        {
                            new IfCondition()
                            {
                                Condition = new ExpressionEngine().Parse("user.Entries[dialog.index].subject == dialog.acceptCalendarEntry_entrySubject"),
                                Steps = new List<IDialog>(){
                                    new SetProperty(){
                                        Property = "dialog.temp",
                                        Value = new ExpressionEngine().Parse("user.Entries[dialog.index]")                                        
                                    },
                                    new SetProperty(){
                                        Property = "dialog.temp.accept",
                                        Value = new ExpressionEngine().Parse("'accepted'") // TODO this would not set the property
                                    },
                                    new EditArray(){
                                        Value = new ExpressionEngine().Parse("dialog.temp"),
                                        ArrayProperty = "dialog.afterAccepted",
                                        ChangeType = EditArray.ArrayChangeType.Push
                                    },
                                    new SendActivity("[AcceptReadBack]")
                                },
                                ElseSteps = new List<IDialog>(){
                                    new EditArray(){
                                        Value = new ExpressionEngine().Parse("user.Entries[dialog.index]"),
                                        ArrayProperty = "dialog.afterAccepted",
                                        ChangeType = EditArray.ArrayChangeType.Push
                                    }
                                }
                            }
                        }
                    },

                    new SetProperty(){
                        Property = "user.Entries",
                        Value = new ExpressionEngine().Parse("dialog.afterAccepted")
                    },

                    new SendActivity("[Welcome-Actions]"),
                    new EndDialog()
                },
                Rules = new List<IRule>()
                {
                    new IntentRule("Cancel")
                    {
                        Steps = new List<IDialog>()
                        {
                            new ConfirmInput()
                            {
                                Property = "turn.cancelConfirmation",
                                Prompt = new ActivityTemplate("[ConfirmCancellation]")
                            },
                            new IfCondition()
                            {
                                Condition = new ExpressionEngine().Parse("turn.cancelConfirmation == true"),
                                Steps = new List<IDialog>()
                                {
                                    new SendActivity("[CancelCreateMeeting]"),
                                    new EndDialog()
                                },
                                ElseSteps = new List<IDialog>()
                                {
                                    new SendActivity("[HelpPrefix], let's get right back to scheduling a meeting.")
                                }
                            }
                        }
                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(acceptCalendarEntry);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }

    }
}
