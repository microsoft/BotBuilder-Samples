using System.Collections.Generic;
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
                    new BeginDialog(nameof(OAuthPromptDialog)),
                    new HttpRequest(){
                        Url = "https://graph.microsoft.com/v1.0/me/calendarview?startdatetime={utcNow()}&enddatetime={addDays(utcNow(), 1)}",
                        Method = HttpRequest.HttpMethod.GET,
                        Headers =  new Dictionary<string, string>()
                        {
                            ["Authorization"] = "Bearer {user.token.Token}",
                        },
                        Property = "dialog.AcceptCalendarEntry_graphAll"
                    },

                    // Handle case where there are no items in todo list
                    new IfCondition()
                    {
                        Condition = new ExpressionEngine().Parse("dialog.AcceptCalendarEntry_graphAll == null || count(dialog.AcceptCalendarEntry_graphAll) <= 0"),
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[AccpetEmptyList]"),
                            new SendActivity("[Welcome-Actions]"),
                            new EndDialog()
                        }
                    },

                    // show all the entries we have
                    new BeginDialog(nameof(FindCalendarEntry)),
                    new TextInput()
                    {
                        Property = "dialog.AcceptCalendarEntry_entrySubject",
                        Prompt = new ActivityTemplate("[GetEntryTitleToAccept]"),
                    },

                    // check each entry to determine whether we have a match
                    new Foreach()
                    {
                        ListProperty = new ExpressionEngine().Parse("dialog.AcceptCalendarEntry_graphAll.value"),
                        Steps = new List<IDialog>()
                        {
                            new IfCondition()
                            {
                                Condition = new ExpressionEngine().Parse("dialog.AcceptCalendarEntry_graphAll.value[dialog.index].subject == dialog.AcceptCalendarEntry_entrySubject"),
                                Steps = new List<IDialog>()
                                {
                                    // we cannot a entry if we are the origanizer
                                    new IfCondition()
                                    {
                                        Condition = new ExpressionEngine().Parse("dialog.AcceptCalendarEntry_graphAll.value[dialog.index].isOrganizer != true"),
                                        Steps = new List<IDialog>()
                                        {
                                            new HttpRequest()
                                            {
                                                Property = "user.acceptResponse",
                                                Method = HttpRequest.HttpMethod.POST,
                                                Url = "https://graph.microsoft.com/v1.0/me/events/{dialog.AcceptCalendarEntry_graphAll.value[dialog.index].id}/accept",
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
                            }
                        }
                    },
                    new SendActivity("[Welcome-Actions]"),
                    new EndDialog()
                },
                Rules = new List<IRule>()
                {
                    new IntentRule("Cancel")
                    {
                        Steps = new List<IDialog>()
                        {
                            new ConfirmInput()
                            {
                                Property = "turn.cancelConfirmation",
                                Prompt = new ActivityTemplate("[ConfirmCancellation]")
                            },
                            new IfCondition()
                            {
                                Condition = new ExpressionEngine().Parse("turn.cancelConfirmation == true"),
                                Steps = new List<IDialog>()
                                {
                                    new SendActivity("[CancelCreateMeeting]"),
                                    new EndDialog()
                                },
                                ElseSteps = new List<IDialog>()
                                {
                                    new SendActivity("[HelpPrefix], let's get right back to scheduling a meeting.")
                                }
                            }
                        }
                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(acceptCalendarEntry);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }

    }
}
