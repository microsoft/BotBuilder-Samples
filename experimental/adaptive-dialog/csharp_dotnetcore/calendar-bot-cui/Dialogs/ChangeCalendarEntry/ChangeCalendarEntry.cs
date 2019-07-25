using System.Collections.Generic;
using System.Globalization;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.LanguageGeneration;
using Newtonsoft.Json.Linq;

/// <summary>
/// This dialog will accept all the calendar entris if they have the same subject
/// </summary>
namespace Microsoft.BotBuilderSamples
{
    public class ChangeCalendarEntry : ComponentDialog
    {
        public ChangeCalendarEntry()
            : base(nameof(ChangeCalendarEntry))
        {
            // Create instance of adaptive dialog. 
            var acceptCalendarEntry = new AdaptiveDialog("change")
            {
                Generator = new ResourceMultiLanguageGenerator("ChangeCalendarEntry.lg"),
                Steps = new List<IDialog>()
                {
                    new IfCondition()
                    {
                        Condition = "user.focusedMeeting == null",
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[emptyFocusedMeeting]"),
                            new SetProperty()
                            {
                                Property = "user.FindCalendarEntry_pageIndex",// index must be set to zero
                                Value = "0" // in case we have not entered FindCalendarEntry from RootDialog
                            },
                            new BeginDialog("FindCalendarEntry")
                        }
                    },
                    new IfCondition()
                    {
                        Condition = "user.focusedMeeting == null",
                        Steps = new List<IDialog>(){
                            new SendActivity("You cannot accept any meetings because your calendar is empty"),
                            new EndDialog()
                        }
                    },
                    new SendActivity("[detailedEntryTemplate(user.focusedMeeting)]"),
                    new ConfirmInput()
                    {
                        Property = "turn.ChangeCalendarEntry_ConfirmChoice",
                        Prompt = new ActivityTemplate("Are you sure you want to update this event?"),
                        InvalidPrompt = new ActivityTemplate("Please Say Yes/No."),
                    },
                    new IfCondition()
                    {
                        Condition = "turn.ChangeCalendarEntry_ConfirmChoice",
                        Steps = new List<IDialog>()
                        {
                            new DateTimeInput()
                            {
                                Property = "dialog.ChangeCalendarEntry_startTime",
                                Prompt = new ActivityTemplate("[GetStartTime]")
                            },
                            //new SendActivity("{dialog.ChangeCalendarEntry_startTime}"),//DEBUG
                            new HttpRequest()
                            {
                                Property = "user.updateResponse",
                                Method = HttpRequest.HttpMethod.PATCH,
                                Url = "https://graph.microsoft.com/v1.0/me/events/{user.focusedMeeting.id}",
                                Headers =  new Dictionary<string, string>()
                                {
                                    ["Authorization"] = "Bearer {user.token.Token}",
                                },
                                Body = JObject.Parse(@"{
                                    'start': {
                                        'dateTime': '{formatDateTime(dialog.ChangeCalendarEntry_startTime[0].value, \'yyyy-MM-ddTHH:mm:ss\')}', 
                                        'timeZone': 'UTC'
                                    }
                                }")
                            },
                            new IfCondition()
                            {
                                Condition = "user.updateResponse.error == null",
                                Steps = new List<IDialog>
                                {
                                    new SendActivity("[UpdateCalendarEntryReadBack]")
                                },
                                ElseSteps = new List<IDialog>
                                {
                                    new SendActivity("[UpdateCalendarEntryFailed]")
                                }
                            },
                        }
                    },
                    // we cannot accept a entry if we are the origanizer
                    
                    new SendActivity("[Welcome-Actions]"),
                    new EndDialog()
                },
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(acceptCalendarEntry);

            // The initial child Dialog to run.
            InitialDialogId = "change";
        }
    }
}
