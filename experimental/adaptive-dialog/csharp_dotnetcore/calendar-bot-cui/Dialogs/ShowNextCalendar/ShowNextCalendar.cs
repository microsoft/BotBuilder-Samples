using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.LanguageGeneration;

/// <summary>
/// This dialog will prompt user to log into calendar account
/// </summary>
namespace Microsoft.CalendarSample
{
    public class ShowNextCalendar : ComponentDialog
    {
        public ShowNextCalendar()
            : base(nameof(ShowNextCalendar))
        {
            var showNextCalendar = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new ResourceMultiLanguageGenerator("ShowNextCalendar.lg"),
                Steps = new List<IDialog>() {
                    new BeginDialog(nameof(OAuthPromptDialog)),
                    new HttpRequest(){
                        Url = "https://graph.microsoft.com/v1.0/me/calendarview?startdatetime={utcNow()}&enddatetime={addDays(utcNow(),7)}", // next 7 days
                        Method = HttpRequest.HttpMethod.GET,
                        Headers =  new Dictionary<string, string>()
                        {
                            ["Authorization"] = "Bearer {user.token.Token}",
                        },
                        Property = "dialog.ShowNextCalendar_graphAll" // sorted by start time already. Sweat!
                    },
                    new IfCondition()
                    {
                        Condition = "dialog.ShowNextCalendar_graphAll != null", // to make sure that we have the next
                        Steps = new List<IDialog>()
                        {
                            new SetProperty()
                            {
                                Value = "dialog.ShowNextCalendar_graphAll.value[0]",
                                Property = "dialog.nextFound"
                            }
                        },
                        ElseSteps = new List<IDialog>()
                        {
                            new SendActivity("[NoNext]"),
                            new SendActivity("[Welcome-Actions]"),
                            new EndDialog()
                        }
                    },
                    new SendActivity("[NextEventPre]"),
                    new SendActivity("[detailedEntryTemplate(dialog.nextFound)]"),// simple template now
                    new SendActivity("[Welcome-Actions]"),
                    new EndDialog()
                }
            };
            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(showNextCalendar);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }
    }
}
