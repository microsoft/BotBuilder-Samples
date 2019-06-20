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
    public class FindCalendarWho : ComponentDialog
    {
        public FindCalendarWho()
            : base(nameof(FindCalendarWho))
        {
            // Create instance of adaptive dialog. 
            var findCalendarWho = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new ResourceMultiLanguageGenerator("FindCalendarWho.lg"),
                Steps = new List<IDialog>()
                {
                    // Handle case where there are no items in todo list
                    new IfCondition()
                    {
                        Condition = new ExpressionEngine().Parse("user.Entries == null || count(user.Entries) <= 0"),
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[ViewEmptyList]"),
                            new SendActivity("[Welcome-Actions]"),
                            new EndDialog()
                        }
                    },

                    // new SaveEntity("@Subject[0]", "user.findCalendarWho_entryName"),                    
                    // new CodeStep(GetToDoTitleToDelete),

                    //new IfCondition()
                    //{
                    //    Steps = new List<IDialog>()
                    //    {
                    //        // First show the current list of Todos
                    new BeginDialog(nameof(FindCalendarEntry)),
                    new TextInput()
                    {
                        Property = "user.findCalendarWho_entryName",
                        Prompt = new ActivityTemplate("[GetPersonName]"),
                    },
                    //    }
                    //},

                    new InitProperty()
                    {
                        Property = "user.findCalendarWho_found",
                        Type = "Array"
                    },

                    new Foreach()
                    {
                        ListProperty = new ExpressionEngine().Parse("user.Entries"),
                        Steps = new List<IDialog>()
                        {
                            new IfCondition()
                            {
                                Condition = new ExpressionEngine().Parse("user.Entries[dialog.index].personname == user.findCalendarWho_entryName"),
                                Steps = new List<IDialog>(){
                                    new EditArray()
                                    {
                                        Value = new ExpressionEngine().Parse("user.Entries[dialog.index]"),
                                        ArrayProperty = "user.findCalendarWho_found",
                                        ChangeType = EditArray.ArrayChangeType.Push
                                    },
                                }
                            }
                        }
                    },

                    new IfCondition()
                    {
                        Condition = new ExpressionEngine().Parse("user.findCalendarWho_found != null && count(user.findCalendarWho_found) > 0"),
                        Steps = new List<IDialog>(){
                            new SendActivity("[ViewEntries]"),
                            new SendActivity("[Welcome-Actions]"),
                            new EndDialog()
                        },
                        ElseSteps = new List<IDialog>(){
                            new SendActivity("We could not find any entries, sorry"),
                            new SendActivity("[Welcome-Actions]"),
                            new EndDialog()
                        }

                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(findCalendarWho);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }
    }
}
