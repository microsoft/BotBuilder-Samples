using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions;
using Microsoft.Bot.Builder.Expressions.Parser;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Bot.Connector.Authentication;
using Newtonsoft.Json.Linq;

/// <summary>
/// Known bug
/// 1. how to wrap up all the information into one unit (subject, time, location and other stuff into one entry),
/// and put it into the user.entries // use user.meetingInfo saveproperty
/// 2. what if user wants to pass some information // use other code
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
                    // every input will be detected through LUIS first from root dialog
                    // once matched, the procedure will add another layer of dialog
                    // not matched. will skip the root dialog, and taken as input
                    new TextInput()
                    {
                        Property = "dialog.createCalendarEntry.Subject",
                        Prompt = new ActivityTemplate("[GetSubject]")
                    },
                    new TextInput()
                    {
                        Property = "dialog.createCalendarEntry.FromTime",
                        Prompt = new ActivityTemplate("[GetFromTime]")
                    },
                    new TextInput()
                    {
                        Property = "dialog.createCalendarEntry.ToTime",
                        Prompt = new ActivityTemplate("[GetToTime]")
                    },
                    new TextInput()
                    {
                        Property = "dialog.createCalendarEntry.PersonName",
                        Prompt = new ActivityTemplate("[GetPersonName]")
                    },
                    //new TextInput()
                    //{
                    //    Property = "dialog.createCalendarEntry.FromDate",
                    //    Prompt = new ActivityTemplate("[GetFromDate]")
                    //},
                    //new TextInput()
                    //{
                    //    Property = "dialog.createCalendarEntry.DestinationCalendar",
                    //    Prompt = new ActivityTemplate("[GetDestinationCalendar]")
                    //},
                    //new TextInput()
                    //{
                    //    Property = "dialog.createCalendarEntry.ToDate",
                    //    Prompt = new ActivityTemplate("[GetToDate]")
                    //},
                    // uncomment to prompt user to enter more information
                    //new TextInput()
                    //{
                    //    Property = "dialog.createCalendarEntry.Location",
                    //    Prompt = new ActivityTemplate("[GetLocation]")
                    //},
                    //new TextInput()
                    //{
                    //    Property = "dialog.createCalendarEntry.MeetingRoom",
                    //    Prompt = new ActivityTemplate("[GetMeetingRoom]")
                    //},
                    //new IfCondition{
                    //    Condition = new ExpressionEngine().Parse("dialog.createCalendarEntry.FromDate != null && dialog.createCalendarEntry.ToDate == null"),
                    //    Steps = new List<IDialog>()
                    //    {
                    //        new TextInput()
                    //        {
                    //            Property = "dialog.createCalendarEntry.Duration",
                    //            Prompt = new ActivityTemplate("[GetDuration]")
                    //        }
                    //  }
                    //},

                    new OAuthPrompt("OAuthPrompt",
                        new OAuthPromptSettings()
                        {
                            Text = "Please log in to your calendar account",
                            ConnectionName = "msgraph",
                            Title = "Sign in",
                        }
                    ){
                        Property = "dialog.token"// not sure it is here or not
                    },
                    new HttpRequest(){
                        Property = "user.createResponse",
                        Method = HttpRequest.HttpMethod.POST,
                        Url = "https://graph.microsoft.com/v1.0/me/events",
                        Headers =  new Dictionary<string, string>(){
                            ["Authorization"] = "Bearer {dialog.token.Token}",
                        },
                        Body = JObject.Parse(@"{
                            'subject': '{dialog.createCalendarEntry.Subject}',
                            'attendees': [
                              {
                                'emailAddress': {
                                  'address': '{dialog.createCalendarEntry.PersonName}'
                                }
                                }
                            ],
                            'start': {
                              'dateTime': '{dialog.createCalendarEntry.FromTime}',
                              'timeZone': 'UTC'
                            },
                            'end': {
                              'dateTime': '{dialog.createCalendarEntry.ToTime}',
                              'timeZone': 'UTC'
                            }
                        }")
                    },
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

        private static IRecognizer CreateRecognizer()//TODO this would not recognize the intends
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

