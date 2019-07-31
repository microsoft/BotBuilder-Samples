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
namespace Microsoft.CalendarSample
{
    public class FindCalendarEntry : ComponentDialog
    {
        private static IConfiguration Configuration;
        public FindCalendarEntry(IConfiguration configuration)
            : base(nameof(FindCalendarEntry))
        {
            Configuration = configuration;
            // Create instance of adaptive dialog. 
            var findCalendarEntry = new AdaptiveDialog("view")
            {
                Recognizer = CreateRecognizer(),
                Generator = new ResourceMultiLanguageGenerator("FindCalendarEntry.lg"),
                Steps = new List<IDialog>()
                {
                    new BeginDialog(nameof(ShowAllMeetingDialog)),
                    new ConfirmInput(){
                        Property = "turn.FindCalendarEntry_ConfirmChoice",
                        Prompt = new ActivityTemplate("[OverviewAgain]"),
                        InvalidPrompt = new ActivityTemplate("[YesOrNo]"),
                    },
                    new IfCondition()
                    {
                        Condition = "turn.FindCalendarEntry_ConfirmChoice",
                        Steps = new List<IDialog>()
                        {
                            new RepeatDialog()
                        },
                        ElseSteps = new List<IDialog>()
                        {
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
                    }
                }
            };
            
            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(findCalendarEntry);
            findCalendarEntry.AddDialog(
                new List<Dialog> {
                    new ShowAllMeetingDialog(Configuration)
                });

            // The initial child Dialog to run.
            InitialDialogId = "view";
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
