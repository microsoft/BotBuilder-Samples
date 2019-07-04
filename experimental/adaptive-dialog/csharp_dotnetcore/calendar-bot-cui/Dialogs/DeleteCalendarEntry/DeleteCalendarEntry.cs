using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
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

                    new BeginDialog(nameof(OAuthPromptDialog)),
                    new HttpRequest()
                    {
                        Url = "https://graph.microsoft.com/v1.0/me/calendarview?startdatetime={utcNow()}&enddatetime={addDays(utcNow(), 1)}",
                        Method = HttpRequest.HttpMethod.GET,
                        Headers = new Dictionary<string, string>()
                        {
                            ["Authorization"] = "Bearer {user.token.Token}",
                        },
                        Property = "dialog.DeleteCalendarEntry_graphAll"
                    },

                    // Handle case where there are no items in todo list
                    new IfCondition()
                    {
                        Condition = "dialog.DeleteCalendarEntry_graphAll == null || count(dialog.DeleteCalendarEntry_graphAll) <= 0",
                        Steps = new List<IDialog>()
                            {
                                new SendActivity("[DeleteEmptyList]"),
                                new SendActivity("[Welcome-Actions]"),
                                new EndDialog()
                            }
                    },

                    new BeginDialog(nameof(FindCalendarEntry)),
                    new TextInput()
                    {
                        Property = "dialog.DeleteCalendarEntry_entrySubject",
                        Prompt = new ActivityTemplate("[GetEntryTitleToDelete]"),
                    },

                    new Foreach()
                    {
                        ListProperty = "dialog.DeleteCalendarEntry_graphAll.value",
                        Steps = new List<IDialog>()
                            {
                                new IfCondition()
                                {
                                    Condition = "dialog.DeleteCalendarEntry_graphAll.value[dialog.index].subject == dialog.DeleteCalendarEntry_entrySubject",
                                    Steps = new List<IDialog>()
                                    {
                                        new HttpRequest(){
                                                Property = "user.deleteResponse",
                                                Method = HttpRequest.HttpMethod.POST, // BUG HttpRequest does not support delete now
                                                Url = "https://graph.microsoft.com/v1.0/me/events/{dialog.DeleteCalendarEntry_graphAll.value[dialog.index].id}/delete",
                                                Headers =  new Dictionary<string, string>()
                                                {
                                                    ["Authorization"] = "Bearer {dialog.token.Token}",
                                                }
                                            },
                                        new SendActivity("[DeleteReadBack]")
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
                                Condition = "turn.cancelConfirmation == true",
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
            AddDialog(deleteCalendarEntry);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }
    }
}
