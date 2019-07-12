using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions.Parser;
using Microsoft.Bot.Builder.LanguageGeneration;

/// <summary>
/// This dialog will accept all the calendar entris if they have the same subject
/// </summary>
namespace Microsoft.BotBuilderSamples
{
    public class AcceptCalendarEntry : ComponentDialog
    {
        public AcceptCalendarEntry()
            : base(nameof(AcceptCalendarEntry))
        {
            // Create instance of adaptive dialog. 
            var acceptCalendarEntry = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new ResourceMultiLanguageGenerator("AcceptCalendarEntry.lg"),
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
                        Property = "turn.AcceptCalendarEntry_ConfirmChoice",
                        Prompt = new ActivityTemplate("Are you sure you want to accept this event?"),
                        InvalidPrompt = new ActivityTemplate("Please Say Yes/No."),
                    },
                    new IfCondition()
                    {
                        Condition = "turn.AcceptCalendarEntry_ConfirmChoice",
                        Steps = new List<IDialog>()
                        {
                            new IfCondition() // we cannot accept a entry if we are the origanizer
                            {
                                Condition = "user.focusedMeeting.isOrganizer != true",
                                Steps = new List<IDialog>()
                                {
                                    new HttpRequest()
                                    {
                                        Property = "user.acceptResponse",
                                        Method = HttpRequest.HttpMethod.POST,
                                        Url = "https://graph.microsoft.com/v1.0/me/events/{user.focusedMeeting.id}/accept",
                                        Headers =  new Dictionary<string, string>()
                                        {
                                            ["Authorization"] = "Bearer {dialog.token.Token}",
                                        }
                                    },
                                    new SendActivity("[AcceptReadBack]")
                                },
                                ElseSteps = new List<IDialog>(){
                                    new SendActivity("Your request can't be completed. You can't respond to this meeting because you're the meeting organizer.")
                                }
                            }
                        }
                    },
                    new SendActivity("[Welcome-Actions]"),
                    new EndDialog()
                 },
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(acceptCalendarEntry);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }
    }
}
