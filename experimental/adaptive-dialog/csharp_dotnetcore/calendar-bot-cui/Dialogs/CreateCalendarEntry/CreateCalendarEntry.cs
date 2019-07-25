using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
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
        private static IConfiguration Configuration;
        public CreateCalendarEntry(IConfiguration configuration)
            : base(nameof(CreateCalendarEntry))
        {
            Configuration = configuration;
            var createCalendarEntry = new AdaptiveDialog("create")
            {
                //Recognizer = CreateRecognizer(),
                Generator = new ResourceMultiLanguageGenerator("CreateCalendarEntry.lg"),
                Steps = new List<IDialog>()
                {

                    new BeginDialog(nameof(OAuthPromptDialog)),
                    new SetProperty()
                    {
                        Value = "@FromTime",
                        Property = "dialog.CreateCalendarEntry_FromTime"
                    },
                    new SetProperty(){
                        Value = "@ToTime",
                        Property = "dialog.CreateCalendarEntry_ToTime"
                    },
                    new SetProperty(){
                        Value = "@Location",
                        Property = "dialog.CreateCalendarEntry_Location"
                    },
                    new SetProperty(){
                        Value = "@Subject",
                        Property = "dialog.CreateCalendarEntry_Subject"
                    },
                    new SetProperty(){ // if not null, then will not ask add another one until no
                        Value = "@personName",
                        Property = "user.CreateCalendarEntry_PersonName"
                    },
                    // add contact flow
                    new SetProperty()
                    {
                        Property = "user.AddContactDialog_pageIndex",// 0-based
                        Value = "0"
                    },
                    new SetProperty()
                    {
                        Property = "user.finalContact",
                        Value = "''"
                    },
                    // for multiple contacts use only
                    //new IfCondition(){
                    //    Condition = "user.CreateCalendarEntry_PersonName == null",
                    //    Steps = new List<IDialog>()
                    //    {
                    //        // add contact flow
                    //        new SetProperty()
                    //        {
                    //            Property = "user.repeatFlag",
                    //            Value = "true"
                    //        },
                    //    },
                    //    ElseSteps = new List<IDialog>()
                    //    {
                    //         new SetProperty()
                    //        {
                    //            Property = "user.repeatFlag",
                    //            Value = "true"
                    //        },
                    //    }
                    //},
                    new BeginDialog("AddContactDialog"),
                    // only for multiple contacts user
                    //new SetProperty()
                    //{
                    //    Property = "user.finalContact",
                    //    Value = "substring(user.finalContact, 0, length(user.finalContact) - 1)"
                    //},
                    //new SetProperty()
                    //{
                    //     Property = "user.finalContact",
                    //     Value = "concat('[', user.finalContact, ']')"
                    //},
                    // new saveproperty not usable TODO
                    new TextInput()
                    {
                        Property = "dialog.CreateCalendarEntry_Subject",
                        Prompt = new ActivityTemplate("[GetSubject]")
                    },
                    new DateTimeInput()
                    {
                        Property = "dialog.CreateCalendarEntry_FromTime",
                        Prompt = new ActivityTemplate("[GetFromTime]")
                    },
                    new DateTimeInput()
                    {
                        Property = "dialog.CreateCalendarEntry_ToTime",
                        Prompt = new ActivityTemplate("[GetToTime]")
                    },
                    new TextInput()
                    {
                        Property = "dialog.CreateCalendarEntry_Location",
                        Prompt = new ActivityTemplate("[GetLocation]")
                    },
                    new SendActivity("[CreateCalendarDetailedEntryReadBack]"),
                    new ConfirmInput(){
                        Property = "turn.CreateCalendarEntry_ConfirmChoice",
                        Prompt = new ActivityTemplate("Is Your Information Correct?"),
                        InvalidPrompt = new ActivityTemplate("Please Say Yes/No."),
                    },
                    // to post our latest update to our calendar
                    new IfCondition()
                    {
                        Condition = "turn.CreateCalendarEntry_ConfirmChoice",
                        Steps = new List<IDialog>(){
                            new HttpRequest()
                            {
                                Property = "dialog.createResponse",
                                Method = HttpRequest.HttpMethod.POST,
                                Url = "https://graph.microsoft.com/v1.0/me/events",
                                Headers =  new Dictionary<string, string>(){
                                    ["Authorization"] = "Bearer {user.token.Token}",
                                },//TODO bugs exists here, because user.finalContact cannot be correctly placed and replaced
                                Body = JObject.Parse(@"{
                                    'subject': '{dialog.CreateCalendarEntry_Subject}',
                                    'attendees': [
                                        {
                                            'emailAddress':
                                            {
                                                'address':'{user.finalContact}'
                                            }
                                        }
                                    ],
                                    'location': {
                                        'displayName': '{dialog.CreateCalendarEntry_Location}',
                                    },
                                    'start': {
                                        'dateTime': '{formatDateTime(dialog.CreateCalendarEntry_FromTime[0].value, \'yyyy-MM-ddTHH:mm:ss\')}',
                                        'timeZone': 'UTC'
                                    },
                                    'end': {
                                        'dateTime': '{formatDateTime(dialog.CreateCalendarEntry_ToTime[0].value, \'yyyy-MM-ddTHH:mm:ss\')}',
                                        'timeZone': 'UTC'
                                    }
                                }")
                                //Body = JObject.Parse(@"{
                                //    'subject': '{dialog.CreateCalendarEntry_Subject}',
                                //    'attendees': '{user.finalContact}',
                                //    'location': {
                                //        'displayName': '{dialog.CreateCalendarEntry_Location}',
                                //    },
                                //    'start': {
                                //        'dateTime': '{formatDateTime(dialog.CreateCalendarEntry_FromTime[0].value, \'yyyy-MM-ddTHH:mm:ss\')}',
                                //        'timeZone': 'UTC'
                                //    },
                                //    'end': {
                                //        'dateTime': '{formatDateTime(dialog.CreateCalendarEntry_ToTime[0].value, \'yyyy-MM-ddTHH:mm:ss\')}',
                                //        'timeZone': 'UTC'
                                //    }
                                //}")
                            }
                        },
                        ElseSteps = new List<IDialog>(){
                            new SendActivity("Sure! let start over!"),
                            new RepeatDialog()
                        }                        
                    },
                    new IfCondition
                    {
                        Condition = "dialog.createResponse.error == null",
                        Steps = new List<IDialog>
                        {
                            new SendActivity("[CreateCalendarEntryReadBack]")
                        },
                        ElseSteps = new List<IDialog>
                        {
                            new SendActivity("{dialog.createResponse}"),
                            new SendActivity("[CreateCalendarEntryFailed]")
                        },
                    },
                    new SendActivity("[Welcome-Actions]"),
                    new EndDialog()
                },

                // note: every input will be detected through this layer first from root dialog
                // once matched. Otherwise, the input will be thrown to the uppser layer.
                //Rules = new List<IRule>()
                //{
                //    new IntentRule("Help")
                //    {
                //        Steps = new List<IDialog>()
                //        {
                //            new SendActivity("[HelpCreateMeeting]")
                //        }
                //    },
                //    new IntentRule("Cancel")
                //    {
                //        Steps = new List<IDialog>()
                //        {
                //                new SendActivity("[CancelCreateMeeting]"),
                //                new EndDialog()
                //        }
                //    },
                //    new IntentRule("ShowPrevious")
                //    {
                //        Steps = new List<IDialog>()
                //        {
                //        new IfCondition()
                //        {
                //            Condition = " 0 < user.CreateCalendarEntry_pageIndex",
                //            Steps = new List<IDialog>()
                //            {
                //                new SetProperty()
                //                {
                //                    Property = "user.CreateCalendarEntry_pageIndex",
                //                    Value = "user.CreateCalendarEntry_pageIndex - 1"
                //                },
                //                new RepeatDialog()
                //            },
                //            ElseSteps = new List<IDialog>()
                //            {
                //                new SendActivity("This is already the first page!"),
                //                new RepeatDialog()
                //            }
                //        }
                //        } 
                //    },
                //    new IntentRule("ShowNext")
                //    {
                //        Steps = new List<IDialog>(){
                //            new IfCondition()
                //            {
                //                Condition = "user.CreateCalendarEntry_pageIndex * 3 + 3 < count(dialog.matchedEmails) ",
                //                Steps = new List<IDialog>()
                //                {
                //                    new SetProperty()
                //                    {
                //                        Property = "user.CreateCalendarEntry_pageIndex",
                //                        Value = "user.CreateCalendarEntry_pageIndex + 1"
                //                    },
                //                    new RepeatDialog()
                //                },
                //                ElseSteps = new List<IDialog>()
                //                {
                //                    new SendActivity("This is already the last page!"),
                //                    new RepeatDialog()
                //                }
                //            }
                //        }
                //    }
                //}
            };
            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(createCalendarEntry);
            createCalendarEntry.AddDialog(
                new List<Dialog>(){
                new AddContactDialog(Configuration)
            });

            // The initial child Dialog to run.
            InitialDialogId = "create";
        }


        //public static IRecognizer CreateRecognizer()
        //{
        //    if (string.IsNullOrEmpty(Configuration["LuisAppIdGeneral"]) || string.IsNullOrEmpty(Configuration["LuisAPIKeyGeneral"]) || string.IsNullOrEmpty(Configuration["LuisAPIHostNameGeneral"]))
        //    {
        //        throw new Exception("Your LUIS application is not configured. Please see README.MD to set up a LUIS application.");
        //    }
        //    return new LuisRecognizer(new LuisApplication()
        //    {
        //        Endpoint = Configuration["LuisAPIHostNameGeneral"],
        //        EndpointKey = Configuration["LuisAPIKeyGeneral"],
        //        ApplicationId = Configuration["LuisAppIdGeneral"]
        //    });
        //}

        //private static IRecognizer CreateRecognizer()
        //{
        //    return new RegexRecognizer()
        //    {
        //        Intents = new Dictionary<string, string>()
        //        {
        //            { "Help", "(?i)help" },
        //            {  "Cancel", "(?i)cancel|never mind"}
        //        }
        //    };
        //}
    }
}
