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
using Microsoft.Bot.Builder.Dialogs.Choices;

/// <summary>
/// This dialog will show all the calendar entries.
/// </summary>
namespace Microsoft.BotBuilderSamples
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
                            new SendActivity("[entryTemplate(dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3], " +
                                "dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3 + 1]," +
                                "dialog.ShowAllMeetingDialog_GraphAll.value[user.ShowAllMeetingDialog_pageIndex * 3 + 2])]"),// TODO only simple card right now, will use fancy card then\
                            new TextInput(){
                                Property = "turn.ShowAllMeetingDialog_Choice",
                                Prompt = new ActivityTemplate("Please enter your choice")
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
                                    new SendActivity("This is already the first page!"),
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
                                Condition = " user.ShowAllMeetingDialog_pageIndex * 3 + 3 < count(dialog.ShowAllMeetingDialog_GraphAll.value)",
                                Steps = new List<IDialog>()
                                {
                                    new SetProperty()
                                    {
                                        Property = "user.ShowAllMeetingDialog_pageIndex",
                                        Value = "user.ShowAllMeetingDialog_pageIndex + 1"
                                    },
                                    new RepeatDialog()
                                },
                                ElseSteps = new List<IDialog>()
                                {
                                    new SendActivity("This is already the last page!"),
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
                                    new SendActivity("{turn.ShowAllMeetingDialog_ordinal}"),
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
                                                            new SendActivity("[viewEmptyEntry]"),
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
                                                            new SendActivity("[viewEmptyEntry]")
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
                                                            new SendActivity("[viewEmptyEntry]")
                                                        }
                                                    }
                                                })
                                        },
                                        Default = new List<IDialog>()
                                        {
                                            new SendActivity("Sorry, I don't know what you mean!"),
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
                                    new SendActivity("{turn.ShowAllMeetingDialog_number}"),
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
                                                            new SendActivity("[viewEmptyEntry]"),
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
                                                            new SendActivity("[viewEmptyEntry]")
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
                                                            new SendActivity("[viewEmptyEntry]")
                                                        }
                                                    }
                                                })
                                        },
                                        Default = new List<IDialog>()
                                        {
                                            new SendActivity("Sorry, I don't know what you mean!"),
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
        //private static IRecognizer CreateRecognizer()
        //{
        //    return new RegexRecognizer()
        //    {
        //        Intents = new Dictionary<string, string>()
        //        {
        //            { "Help", "(?i)help" },
        //            { "Cancel", "(?i)cancel|never mind"},
        //        }
        //    };
        //}

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
