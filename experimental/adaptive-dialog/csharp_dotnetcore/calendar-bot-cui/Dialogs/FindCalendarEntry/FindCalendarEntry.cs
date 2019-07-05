using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.LanguageGeneration;

/// <summary>
/// This dialog will show all the calendar entries.
/// </summary>
namespace Microsoft.BotBuilderSamples
{
    public class FindCalendarEntry : ComponentDialog
    {
        public FindCalendarEntry()
            : base(nameof(FindCalendarEntry))
        {
            // Create instance of adaptive dialog. 
            var findCalendarEntry = new AdaptiveDialog("view")
            {
                Recognizer = CreateRecognizer(),
                Generator = new ResourceMultiLanguageGenerator("FindCalendarEntry.lg"),
                Steps = new List<IDialog>()
                {
                    new BeginDialog(nameof(OAuthPromptDialog)),
                    new HttpRequest() {
                        Url = "https://graph.microsoft.com/v1.0/me/calendarview?startdatetime={utcNow()}&enddatetime={addDays(utcNow(), 1)}",
                        Method = HttpRequest.HttpMethod.GET,
                        Headers = new Dictionary<string, string>()
                        {
                            ["Authorization"] = "Bearer {user.token.Token}",
                        },
                        Property = "dialog.FindCalendarEntry_GraphAll"
                    },
                    // to avoid shoing an empty calendar
                    new IfCondition()
                    {
                        Condition = "dialog.FindCalendarEntry_GraphAll.value != null && count(dialog.FindCalendarEntry_GraphAll.value) > 0",
                        Steps = new List<IDialog>()
                        {   
                            new SendActivity("[entryTemplate(dialog.FindCalendarEntry_GraphAll.value[user.FindCalendarEntry_pageIndex * 3], " +
                                "dialog.FindCalendarEntry_GraphAll.value[user.FindCalendarEntry_pageIndex * 3 + 1]," +
                                "dialog.FindCalendarEntry_GraphAll.value[user.FindCalendarEntry_pageIndex * 3 + 2])]"),// TODO only simple card right now, will use fancy card then\
                            new DeleteProperty
                            {
                                Property = "turn.FindCalendarEntry_Choice"
                            },
                            new ChoiceInput(){
                                Property = "turn.FindCalendarEntry_Choice",
                                Prompt = new ActivityTemplate("[EnterYourChoice]"),
                                Choices = new List<Choice>()
                                {
                                    new Choice("Check The First One"),
                                    new Choice("Check The Second One"),
                                    new Choice("Check The Third One"),
                                    new Choice("Next Page"),
                                    new Choice("Previous Page")
                                }
                            },
                            new SwitchCondition()
                            {
                                Condition = "turn.FindCalendarEntry_Choice",
                                Cases = new List<Case>()
                                {
                                    new Case("Check The First One", new List<IDialog>()
                                        {
                                            new IfCondition(){
                                                Condition = "dialog.FindCalendarEntry_GraphAll.value[user.FindCalendarEntry_pageIndex * 3] != null",
                                                Steps = new List<IDialog>(){
                                                    new SendActivity("[detailedEntryTemplate(dialog.FindCalendarEntry_GraphAll.value[user.FindCalendarEntry_pageIndex * 3])]"),
                                                    new SetProperty()
                                                    {
                                                        Property = "user.focusedMeeting",
                                                        Value = "dialog.FindCalendarEntry_GraphAll.value[user.FindCalendarEntry_pageIndex * 3]"
                                                    }
                                                },
                                                ElseSteps = new List<IDialog>(){
                                                    new SendActivity("[viewEmptyEntry]")
                                                }
                                            },
                                            new RepeatDialog()
                                        }),
                                    new Case("Check The Second One", new List<IDialog>()
                                        {
                                            new IfCondition(){
                                                Condition = "dialog.FindCalendarEntry_GraphAll.value[user.FindCalendarEntry_pageIndex * 3 + 1] != null",
                                                Steps = new List<IDialog>(){
                                                    new SendActivity("[detailedEntryTemplate(dialog.FindCalendarEntry_GraphAll.value[user.FindCalendarEntry_pageIndex * 3 + 1])]"),
                                                    new SetProperty()
                                                    {
                                                        Property = "user.focusedMeeting",
                                                        Value = "dialog.FindCalendarEntry_GraphAll.value[user.FindCalendarEntry_pageIndex * 3 + 1]"
                                                    }
                                                },
                                                ElseSteps = new List<IDialog>(){
                                                    new SendActivity("[viewEmptyEntry]")
                                                }
                                            },
                                            new RepeatDialog()
                                        }),
                                    new Case("Check The Third One", new List<IDialog>()
                                        {
                                            new IfCondition(){
                                                Condition = "dialog.FindCalendarEntry_GraphAll.value[user.FindCalendarEntry_pageIndex * 3 + 2] != null",
                                                Steps = new List<IDialog>(){
                                                    new SendActivity("[detailedEntryTemplate(dialog.FindCalendarEntry_GraphAll.value[user.FindCalendarEntry_pageIndex * 3 + 2])]"),
                                                    new SetProperty()
                                                    {
                                                        Property = "user.focusedMeeting",
                                                        Value = "dialog.FindCalendarEntry_GraphAll.value[user.FindCalendarEntry_pageIndex * 3 + 2]"
                                                    }
                                                },
                                                ElseSteps = new List<IDialog>(){
                                                    new SendActivity("[viewEmptyEntry]")
                                                }
                                            },
                                            new RepeatDialog()
                                        }),
                                    new Case("Next Page", new List<IDialog>()
                                        {
                                            new IfCondition()
                                            {
                                                Condition = "user.FindCalendarEntry_pageIndex < count(dialog.FindCalendarEntry_GraphAll.value) - 1",
                                                Steps = new List<IDialog>()
                                                {
                                                    new SetProperty()
                                                    {
                                                       Property = "user.FindCalendarEntry_pageIndex",
                                                        Value = "user.FindCalendarEntry_pageIndex + 1"
                                                    },
                                                    new RepeatDialog()
                                                },
                                                ElseSteps = new List<IDialog>()
                                                {
                                                    new SendActivity("This is already the last page!"),
                                                    new RepeatDialog()
                                                }
                                            }
                                        }),
                                    new Case("Previous Page", new List<IDialog>()
                                        {
                                            new IfCondition()
                                            {
                                                Condition = " 0 < user.FindCalendarEntry_pageIndex",
                                                Steps = new List<IDialog>()
                                                {
                                                    new SetProperty()
                                                    {
                                                        Property = "user.FindCalendarEntry_pageIndex",
                                                        Value = "user.FindCalendarEntry_pageIndex - 1"
                                                    },
                                                    new RepeatDialog()
                                                },
                                                ElseSteps = new List<IDialog>()
                                                {
                                                    new SendActivity("This is already the first page!"),
                                                    new RepeatDialog()
                                                }
                                            }
                                        }),
                                },
                                Default = new List<IDialog>()
                                {
                                    new SendActivity("Sorry, I don't know what you mean!"),
                                    new EndDialog()
                                }
                            }
                            // read one two three
                            // show details
                            // repeatdialog

                        },
                        ElseSteps = new List<IDialog>
                        {
                            new SendActivity("[NoEntries]"),
                            new SendActivity("[Welcome-Actions]"),
                            new EndDialog()
                        }
                    }
                },
                Rules = new List<IRule>()
                {
                    new IntentRule("Help")
                    {
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[HelpViewMeeting]")
                        }
                    },
                    new IntentRule("Cancel")
                    {
                        Steps = new List<IDialog>()
                        {
                                new SendActivity("[CancelViewMeeting]"),
                                new EndDialog()
                        }
                    }
                }
            };
            
            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(findCalendarEntry);

            // The initial child Dialog to run.
            InitialDialogId = "view";
        }
        private static IRecognizer CreateRecognizer()
        {
            return new RegexRecognizer()
            {
                Intents = new Dictionary<string, string>()
                {
                    { "Help", "(?i)help" },
                    { "Cancel", "(?i)cancel|never mind"},
                }
            };
        }
    }
}
