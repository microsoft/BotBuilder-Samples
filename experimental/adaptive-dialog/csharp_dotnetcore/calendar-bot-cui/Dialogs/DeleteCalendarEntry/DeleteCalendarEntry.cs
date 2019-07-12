using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Expressions.Parser;
using Microsoft.Bot.Builder.LanguageGeneration;

/// <summary>
/// Delete calendar entry is not functioning now because we could not use http.delete
/// </summary>
namespace Microsoft.BotBuilderSamples
{
    public class DeleteCalendarEntry : ComponentDialog
    {
        public DeleteCalendarEntry()
            : base(nameof(DeleteCalendarEntry))
        {
            // Create instance of adaptive dialog. 
            var deleteCalendarEntry = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new ResourceMultiLanguageGenerator("DeleteCalendarEntry.lg"),
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
                        Property = "turn.DeleteCalendarEntry_ConfirmChoice",
                        Prompt = new ActivityTemplate("Are you sure you want to decline this event?"),
                        InvalidPrompt = new ActivityTemplate("Please Say Yes/No."),
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
                                    new SendActivity("Your request can't be completed. You can't respond to this meeting because you're the meeting organizer."),
                                    new HttpRequest()
                                    {
                                        Property = "user.declineResponse",
                                        //Method = HttpRequest.HttpMethod.DELETE, // CANNOT DELETE NOT BECAUSE IT IS NOT USABLE NOW
                                        Url = "https://graph.microsoft.com/v1.0/me/events/{user.focusedMeeting.id}/delete",
                                        Headers =  new Dictionary<string, string>()
                                        {
                                            ["Authorization"] = "Bearer {dialog.token.Token}",
                                        }
                                    },
                                    new SendActivity("Successfully delete your entry!"),
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

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }
    }
}
