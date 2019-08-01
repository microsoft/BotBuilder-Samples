using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;

/// <summary>
/// This dialog will show all the calendar entries.
/// </summary>
namespace Microsoft.CalendarSample
{
    public class ShowAllMeetingDialog : ComponentDialog
    {
        private static IConfiguration Configuration;
        public ShowAllMeetingDialog(IConfiguration configuration)
            : base(nameof(ShowAllMeetingDialog))
        {
            Configuration = configuration;
            // Create instance of adaptive dialog. 
            var showAllMeetingDialog = new AdaptiveDialog("showAll")
            {
                Recognizer = CreateRecognizer(),
                Generator = new ResourceMultiLanguageGenerator("ShowAllMeetingDialog.lg"),
                Steps = new List<IDialog>()
                {
                    new BeginDialog(nameof(OAuthPromptDialog)),
                    new HttpRequest() {
                        Url = "https://graph.microsoft.com/v1.0/me/calendarView?startdatetime={utcNow()}&enddatetime={addDays(utcNow(), 1)}",
                        Method = HttpRequest.HttpMethod.GET,
                        Headers = new Dictionary<string, string>()
                        {
                            ["Authorization"] = "Bearer {user.token.Token}",
                        },
                        Property = "dialog.ShowAllMeetingDialog_GraphAll"
                    },
                    // new SendActivity("{dialog.ShowAllMeetingDialog_GraphAll.value}"),
                    // to avoid shoing an empty calendar & access denied
                    new IfCondition()
                    {
                        Condition = "dialog.ShowAllMeetingDialog_GraphAll.value != null && count(dialog.ShowAllMeetingDialog_GraphAll.value) > 0",
                        Steps = new List<IDialog>()
                        {
                            //new SendActivity("{count(dialog.ShowAllMeetingDialog_GraphAll.value)}"),
                            new IfCondition()
                            {
                                Condition = "(user.ShowAllMeetingDialog_pageIndex*3+2) < count(dialog.ShowAllMeetingDialog_GraphAll.value)",
                                Steps = new List<IDialog>()
                                {
                                    new SendActivity("[stitchedEntryTemplate(dialog.ShowAllMeetingDialog_GraphAll.value, user.ShowAllMeetingDialog_pageIndex*3, user.ShowAllMeetingDialog_pageIndex*3+3)]"),
                                },
                                ElseSteps = new List<IDialog>()
                                {
                                    new SendActivity("[stitchedEntryTemplate(dialog.ShowAllMeetingDialog_GraphAll.value, user.ShowAllMeetingDialog_pageIndex*3, count(dialog.ShowAllMeetingDialog_GraphAll.value))]"),
                                }
                            },// TODO only simple card right now, will use fancy card then
                            new TextInput(){
                                Property = "turn.ShowAllMeetingDialog_Choice",
                                Prompt = new ActivityTemplate("[ChoicePrompt]")
                            }
                        },
                        ElseSteps = new List<IDialog>
                        {
                            new SendActivity("[NoEntries]"),
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
                                new CancelAllDialogs()
                        }
                    },
                    new IntentRule("ShowPrevious")
                    {
                        Steps = new List<IDialog>()
                        {
                            new IfCondition()
                            {
                                Condition = " 0 < user.ShowAllMeetingDialog_pageIndex",
                                Steps = new List<IDialog>()
                                {
                                    new SetProperty()
                                    {
                                        Property = "user.ShowAllMeetingDialog_pageIndex",
                                        Value = "user.ShowAllMeetingDialog_pageIndex - 1"
                                    },
                                    new RepeatDialog()
                                },
                                ElseSteps = new List<IDialog>()
                                {
                                    new SendActivity("[FirstPage]"),
                                    new RepeatDialog()
                                }
                            }
                        }
                    },
                    new IntentRule("ShowNext")
                    {
                        Steps = new List<IDialog>()
                        {
                            new IfCondition()
                            {
                                Condition = " user.ShowAllMeetingDialog_pageIndex*3+3<count(dialog.ShowAllMeetingDialog_GraphAll.value)",
                                Steps = new List<IDialog>()
                                {
                                    new SetProperty()
                                    {
                                        Property = "user.ShowAllMeetingDialog_pageIndex",
                                        Value = "user.ShowAllMeetingDialog_pageIndex+1"
                                    },
                                    new RepeatDialog()
                                },
                                ElseSteps = new List<IDialog>()
                                {
                                    new SendActivity("[LastPage]"),
                                    new RepeatDialog()
                                }
                            }
                        }
                    },
                    new IntentRule("SelectItem")
                    {
                        Steps = new List<IDialog>()
                        {
                            new SetProperty()
                            {
                                Value = "@ordinal",
                                Property = "turn.ShowAllMeetingDialog_ordinal"
                            },
                            new SetProperty()
                            {
                                Value = "@number",
                                Property = "turn.ShowAllMeetingDialog_number"
                            },
                            new IfCondition()
                            {
                                Condition = "turn.ShowAllMeetingDialog_ordinal != null",
                                Steps = new List<IDialog>()
                                {
                                    new SwitchCondition()
                                    {
                                        Condition = "turn.ShowAllMeetingDialog_ordinal",
                                        Cases = new List<Case>()
                                        {
                                            new Case("1", new List<IDialog>()
                                                {
                                                    new IfCondition(){
                                                        Condition = "dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3] != null",
                                                        Steps = new List<IDialog>(){
                                                            new SendActivity("[detailedEntryTemplate(dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3])]"),
                                                            new SetProperty()
                                                            {
                                                                Property = "user.focusedMeeting",
                                                                Value = "dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3]"
                                                            }
                                                        },
                                                        ElseSteps = new List<IDialog>(){
                                                            new SendActivity("[ViewEmptyEntry]"),
                                                        }
                                                    }
                                                }),
                                            new Case("2", new List<IDialog>()
                                                {
                                                    new IfCondition(){
                                                        Condition = "dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3 + 1] != null",
                                                        Steps = new List<IDialog>(){
                                                            new SendActivity("[detailedEntryTemplate(dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3 + 1])]"),
                                                            new SetProperty()
                                                            {
                                                                Property = "user.focusedMeeting",
                                                                Value = "dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3 + 1]"
                                                            }
                                                        },
                                                        ElseSteps = new List<IDialog>(){
                                                            new SendActivity("[ViewEmptyEntry]")
                                                        }
                                                    }
                                                }),
                                            new Case("3", new List<IDialog>()
                                                {
                                                    new IfCondition(){
                                                        Condition = "dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3 + 2] != null",
                                                        Steps = new List<IDialog>(){
                                                            new SendActivity("[detailedEntryTemplate(dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3 + 2])]"),
                                                            new SetProperty()
                                                            {
                                                                Property = "user.focusedMeeting",
                                                                Value = "dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3 + 2]"
                                                            }
                                                        },
                                                        ElseSteps = new List<IDialog>(){
                                                            new SendActivity("[ViewEmptyEntry]")
                                                        }
                                                    }
                                                })
                                        },
                                        Default = new List<IDialog>()
                                        {
                                            new SendActivity("[CannotUnderstand]"),
                                            new EndDialog()
                                        }
                                    },
                                    new EndDialog()
                                }
                            },
                            new IfCondition()
                            {
                                // prevent "select the third one"
                                Condition = "turn.ShowAllMeetingDialog_number != null && turn.ShowAllMeetingDialog_ordinal == null",
                                Steps = new List<IDialog>()
                                {
                                    new SwitchCondition()
                                    {
                                        Condition = "turn.ShowAllMeetingDialog_number",
                                        Cases = new List<Case>()
                                        {
                                            new Case("1", new List<IDialog>()
                                                {
                                                    new IfCondition(){
                                                        Condition = "dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3] != null",
                                                        Steps = new List<IDialog>(){
                                                            new SendActivity("[detailedEntryTemplate(dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3])]"),
                                                            new SetProperty()
                                                            {
                                                                Property = "user.focusedMeeting",
                                                                Value = "dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3]"
                                                            }
                                                        },
                                                        ElseSteps = new List<IDialog>(){
                                                            new SendActivity("[ViewEmptyEntry]"),
                                                        }
                                                    }
                                                }),
                                            new Case("2", new List<IDialog>()
                                                {
                                                    new IfCondition(){
                                                        Condition = "dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3 + 1] != null",
                                                        Steps = new List<IDialog>(){
                                                            new SendActivity("[detailedEntryTemplate(dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3 + 1])]"),
                                                            new SetProperty()
                                                            {
                                                                Property = "user.focusedMeeting",
                                                                Value = "dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3 + 1]"
                                                            }
                                                        },
                                                        ElseSteps = new List<IDialog>(){
                                                            new SendActivity("[ViewEmptyEntry]")
                                                        }
                                                    }
                                                }),
                                            new Case("3", new List<IDialog>()
                                                {
                                                    new IfCondition(){
                                                        Condition = "dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3 + 2] != null",
                                                        Steps = new List<IDialog>(){
                                                            new SendActivity("[detailedEntryTemplate(dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3 + 2])]"),
                                                            new SetProperty()
                                                            {
                                                                Property = "user.focusedMeeting",
                                                                Value = "dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3 + 2]"
                                                            }
                                                        },
                                                        ElseSteps = new List<IDialog>(){
                                                            new SendActivity("[ViewEmptyEntry]")
                                                        }
                                                    }
                                                })
                                        },
                                        Default = new List<IDialog>()
                                        {
                                            new SendActivity("[CannotUnderstand]"),
                                            new EndDialog()
                                        }
                                    },
                                    new EndDialog()
                                }
                            },
                        }

                    }
                }
            };
            
            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(showAllMeetingDialog);

            // The initial child Dialog to run.
            InitialDialogId = "showAll";
        }
    
        public static IRecognizer CreateRecognizer()
        {
            if (string.IsNullOrEmpty(Configuration["LuisAppIdGeneral"]) || string.IsNullOrEmpty(Configuration["LuisAPIKeyGeneral"]) || string.IsNullOrEmpty(Configuration["LuisAPIHostNameGeneral"]))
            {
                throw new Exception("Your LUIS application is not configured. Please see README.MD to set up a LUIS application.");
            }
            return new LuisRecognizer(new LuisApplication()
            {
                Endpoint = Configuration["LuisAPIHostNameGeneral"],
                EndpointKey = Configuration["LuisAPIKeyGeneral"],
                ApplicationId = Configuration["LuisAppIdGeneral"]
            });
        }
    }
}
