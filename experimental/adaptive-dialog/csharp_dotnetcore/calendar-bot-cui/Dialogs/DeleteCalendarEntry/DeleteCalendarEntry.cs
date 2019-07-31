using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Expressions.Parser;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Extensions.Configuration;

/// <summary>
/// Delete calendar entry is not functioning now because we could not use http.delete
/// </summary>
namespace Microsoft.CalendarSample
{
    public class DeleteCalendarEntry : ComponentDialog
    {
        private static IConfiguration Configuration;
        public DeleteCalendarEntry(IConfiguration configuration)
            : base(nameof(DeleteCalendarEntry))
        {
            Configuration = configuration;
            // Create instance of adaptive dialog. 
            var deleteCalendarEntry = new AdaptiveDialog("delete")
            {
                Generator = new ResourceMultiLanguageGenerator("DeleteCalendarEntry.lg"),
                Steps = new List<IDialog>()
                {
                    new SendActivity("[emptyFocusedMeeting]"),
                    new SetProperty()
                    {
                        Property = "user.ShowAllMeetingDialog_pageIndex",// index must be set to zero
                        Value = "0" // in case we have not entered FindCalendarEntry from RootDialog
                    },
                    new BeginDialog("ShowAllMeetingDialog"),
                    new IfCondition()
                    {
                        Condition = "user.focusedMeeting == null",
                        Steps = new List<IDialog>(){
                            new SendActivity("[EmptyCalendar]"),
                            new EndDialog()
                        }
                    },
                    new ConfirmInput()
                    {
                        Property = "turn.DeleteCalendarEntry_ConfirmChoice",
                        Prompt = new ActivityTemplate("[DeclineConfirm]"),
                        InvalidPrompt = new ActivityTemplate("[YesOrNo]"),
                    },
                    new IfCondition()
                    { 
                        Condition = "turn.DeleteCalendarEntry_ConfirmChoice",
                        Steps = new List<IDialog>()
                        {
                            new IfCondition()
                            {
                                Condition = "user.focusedMeeting.isOrganizer != true",// we cannot decline a entry if we are the origanizer
                                Steps = new List<IDialog>()
                                {
                                    new HttpRequest()
                                    {
                                        Property = "user.declineResponse",
                                        Method = HttpRequest.HttpMethod.POST,
                                        Url = "https://graph.microsoft.com/v1.0/me/events/{user.focusedMeeting.id}/decline",
                                        Headers =  new Dictionary<string, string>()
                                        {
                                            ["Authorization"] = "Bearer {user.token.Token}",
                                        }
                                    },
                                    new SendActivity("[DeclineReadBack]")
                                },
                                ElseSteps = new List<IDialog>(){
                                    new SendActivity("[CannotDeclineOrganizer]"),
                                    new HttpRequest()
                                    {
                                        Property = "user.declineResponse",
                                        //Method = HttpRequest.HttpMethod.DELETE, // CANNOT DELETE NOT BECAUSE IT IS NOT USABLE NOW
                                        Url = "https://graph.microsoft.com/v1.0/me/events/{user.focusedMeeting.id}/delete",
                                        Headers =  new Dictionary<string, string>()
                                        {
                                            ["Authorization"] = "Bearer {user.token.Token}",
                                        }
                                    },
                                    new SendActivity("[DeleteReadBack]"),
                                }
                            }
                        }
                    },
                    new SendActivity("[Welcome-Actions]"),
                    new EndDialog()
                },
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(deleteCalendarEntry);
            deleteCalendarEntry.AddDialog(
                new List<Dialog> {
                    new ShowAllMeetingDialog(Configuration)
                });


            // The initial child Dialog to run.
            InitialDialogId = "delete";
        }
    }
}
