using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions.Parser;
using Microsoft.Bot.Builder.LanguageGeneration;
using Newtonsoft.Json.Linq;

/// <summary>
/// This dialog will create a calendar entry by propting user to enter the relevant information,
/// including subject, starting time, ending time, and the email address of the attendee
/// The user can enter only one attendee
/// </summary>
namespace Microsoft.BotBuilderSamples
{
    public class CreateCalendarEntry : ComponentDialog
    {
        public CreateCalendarEntry()
            : base(nameof(CreateCalendarEntry))
        {
            var createCalendarEntry = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Recognizer = CreateRecognizer(),
                Generator = new ResourceMultiLanguageGenerator("CreateCalendarEntry.lg"),
                Steps = new List<IDialog>()
                {
                    new TextInput()
                    {
                        Property = "dialog.CreateCalendarEntry_Subject",
                        Prompt = new ActivityTemplate("[GetSubject]")
                    },
                    new TextInput()
                    {
                        Property = "dialog.CreateCalendarEntry_FromTime",
                        Prompt = new ActivityTemplate("[GetFromTime]")
                    },
                    new TextInput()
                    {
                        Property = "dialog.CreateCalendarEntry_ToTime",
                        Prompt = new ActivityTemplate("[GetToTime]")
                    },
                    new TextInput()
                    {
                        Property = "dialog.CreateCalendarEntry_PersonName",
                        Prompt = new ActivityTemplate("[GetPersonName]")
                    },

                    new OAuthPrompt("OAuthPrompt",
                        new OAuthPromptSettings()
                        {
                            Text = "Please log in to your calendar account",
                            ConnectionName = "msgraph",
                            Title = "Sign in",
                        }
                    ){
                        Property = "dialog.token"
                    },
                    // to post our latest update to our calendar
                    new HttpRequest(){
                        Property = "user.createResponse",
                        Method = HttpRequest.HttpMethod.POST,
                        Url = "https://graph.microsoft.com/v1.0/me/events",
                        Headers =  new Dictionary<string, string>(){
                            ["Authorization"] = "Bearer {dialog.token.Token}",
                        },
                        Body = JObject.Parse(@"{
                            'subject': '{dialog.CreateCalendarEntry_Subject}',
                            'attendees': [
                              {
                                'emailAddress': {
                                  'address': '{dialog.CreateCalendarEntry_PersonName}'
                                }
                                }
                            ],
                            'start': {
                              'dateTime': '{dialog.CreateCalendarEntry_FromTime}',
                              'timeZone': 'UTC'
                            },
                            'end': {
                              'dateTime': '{dialog.CreateCalendarEntry_ToTime}',
                              'timeZone': 'UTC'
                            }
                        }")
                    },
                    // to calculate how many calendar entries in our calendar
                    new HttpRequest(){
                        Url = "https://graph.microsoft.com/v1.0/me/calendarview?startdatetime={utcNow()}&enddatetime={addDays(utcNow(), 1)}",
                        Method = HttpRequest.HttpMethod.GET,
                        Headers =  new Dictionary<string, string>()
                        {
                            ["Authorization"] = "Bearer {dialog.token.Token}",
                        },
                        Property = "dialog.CreateCalendarEntry_GraphAll"
                    },
                    new SendActivity("[CreateCalendarEntryReadBack]"),
                    new EndDialog()
                },
                // note: every input will be detected through LUIS first from root dialog
                // once matched, the procedure will add another layer of dialog
                // not matched. will skip the root dialog, and taken as input
                Rules = new List<IRule>()
                {
                    new IntentRule("Help")
                    {
                    Steps = new List<IDialog>()
                        {
                            new SendActivity("[HelpCreateMeeting]")
                        }
                    },
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
            AddDialog(createCalendarEntry);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }

        private static IRecognizer CreateRecognizer()// BUG this would not recognize the intends
            // i.e by typing "Cancel", we cannot exit the current dialog
        {
            return new RegexRecognizer()
            {
                Intents = new Dictionary<string, string>()
                    {
                        { "Help", "(?i)help" },
                        { "Cancel", "(?i)cancel|never mind" }
                    }
            };
        }
    }
}

