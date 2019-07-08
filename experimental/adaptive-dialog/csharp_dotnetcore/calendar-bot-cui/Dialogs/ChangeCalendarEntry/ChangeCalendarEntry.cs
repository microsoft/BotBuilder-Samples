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
                    // we cannot accept a entry if we are the origanizer
                    new TextInput(){
                        Property = "dialog.ChangeCalendarEntry_startTime",
                        Prompt = new ActivityTemplate("[GetStartTime]")
                    },
                    //new SendActivity("{formatDateTime(dialog.ChangeCalendarEntry_startTime, \"yyyy-MM-ddTHH:mm:ss\")}"),//DEBUG
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
                                'dateTime': '{formatDateTime(dialog.ChangeCalendarEntry_startTime, \'yyyy-MM-ddTHH:mm:ss\')}', 
                                'timeZone': 'UTC'
                            }
                        }")// have some bugs in formatDateTime
                        // cannot delete calendar entry in outlook manually
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
                    new SendActivity("[AcceptReadBack]"),
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
