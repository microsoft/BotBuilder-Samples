using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
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
                            new Foreach()
                            {
                                ListProperty = "dialog.FindCalendarEntry_GraphAll.value",
                                Steps = new List<IDialog>()
                                {
                                    new IfCondition()
                                    {
                                        Condition = "user.FindCalendarEntry_pageIndex * 3 - 1 < dialog.index &&" +
                                            "dialog.index < user.FindCalendarEntry_pageIndex * 3 + 3",
                                        Steps = new List<IDialog>()
                                        {
                                            new SendActivity("[entryTemplate(dialog.FindCalendarEntry_GraphAll.value[dialog.index])]"),// TODO only simple card right now, will use fancy card then\
                                        }
                                    }
                                }
                            },
                            new TextInput()
                            {
                                Property = "dialog.FindCalendarEntry_Choice",
                                Prompt = new ActivityTemplate("[EnterYourChoice]")
                            }
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
                    new IntentRule("NextPage")
                    {
                        Steps = new List<IDialog>()
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
                                    new SendActivity("{user.FindCalendarEntry_pageIndex}"),//DEBUG
                                    new BeginDialog("FindCalendarEntry")
                                },
                                ElseSteps = new List<IDialog>()
                                {
                                    new SendActivity("This is already the last page!"),
                                    new BeginDialog("FindCalendarEntry")
                                }                                
                            }
                        }
                    },
                    new IntentRule("PreviousPage")
                    {
                        Steps = new List<IDialog>()
                        {

                        new IfCondition()
                        {
                            Condition = "user.FindCalendarEntry_pageIndex >= 1",
                            Steps = new List<IDialog>()
                            {
                                new SetProperty()
                                {
                                    Property = "user.FindCalendarEntry_pageIndex",
                                    Value = "user.FindCalendarEntry_pageIndex - 1"
                                },
                                new SendActivity("{user.FindCalendarEntry_pageIndex}"),//DEBUG
                                new BeginDialog("FindCalendarEntry")
                            },
                            ElseSteps = new List<IDialog>()
                            {
                                new SendActivity("This is already the first page!"),
                                new BeginDialog("FindCalendarEntry")
                            }
                        }
                        },
                    },
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
                    { "NextPage", "(?i)next page"},
                    { "PreviousPage", "(?i)previous page"}
                }
            };
        }
    }
}
