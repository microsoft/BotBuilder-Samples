using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Dialogs.Choices;
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
            var createCalendarEntry = new AdaptiveDialog("create")
            {
                Recognizer = CreateRecognizer(),
                Generator = new ResourceMultiLanguageGenerator("CreateCalendarEntry.lg"),
                Steps = new List<IDialog>()
                {

                    new BeginDialog(nameof(OAuthPromptDialog)),
                    new IfCondition()
                    {
                        Condition = "user.CreateSuccess == ture",
                        Steps = new List<IDialog>()
                        {
                            new SetProperty(){
                                Property = "user.CreateSuccess",
                                Value = "false"
                            },
                            new TextInput()
                            {
                                Property = "user.CreateCalendarEntry_PersonName",
                                Prompt = new ActivityTemplate("[GetPersonName]")
                            }
                        }
                    },
                    new HttpRequest(){
                        Property = "dialog.CreateCalendarEntry_UserAll",
                        Method = HttpRequest.HttpMethod.GET,
                        Url = "https://graph.microsoft.com/v1.0/me/contacts",
                        Headers =  new Dictionary<string, string>(){
                            ["Authorization"] = "Bearer {user.token.Token}",
                        }
                    },
                    new IfCondition(){
                        Condition = "dialog.CreateCalendarEntry_UserAll.value != null && count(dialog.CreateCalendarEntry_UserAll.value) > 0",
                        Steps = new List<IDialog>(){
                            new Foreach()
                            {
                                ListProperty = "dialog.CreateCalendarEntry_UserAll.value",
                                Steps = new List<IDialog>()
                                {
                                    new IfCondition()
                                    {
                                        Condition = "contains(dialog.CreateCalendarEntry_UserAll.value[dialog.index].displayName, user.CreateCalendarEntry_PersonName) == true ||" +
                                            "contains(dialog.CreateCalendarEntry_UserAll.value[dialog.index].emailAddresses[0].address, user.CreateCalendarEntry_PersonName) == true",
                                        Steps = new List<IDialog>()
                                        {
                                            new EditArray(){
                                                ArrayProperty = "dialog.matchedEmails",
                                                ChangeType = EditArray.ArrayChangeType.Push,
                                                Value = "dialog.CreateCalendarEntry_UserAll.value[dialog.index].emailAddresses[0].address"
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    new IfCondition()
                    {
                        Condition = "dialog.matchedEmails != null && count(dialog.matchedEmails) > 0",
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[emailTemplate(dialog.matchedEmails[user.CreateCalendarEntry_pageIndex * 3], " +
                                "dialog.matchedEmails[user.CreateCalendarEntry_pageIndex * 3 + 1]," +
                                "dialog.matchedEmails[user.CreateCalendarEntry_pageIndex * 3 + 2])]"),// TODO only simple card right now, will use fancy card then\
                            new DeleteProperty
                            {
                                Property = "turn.CreateCalendarEntry_Choice"
                            },
                            new ChoiceInput(){//DEBUG why style different from FindCAlendarEntry
                                Property = "turn.CreateCalendarEntry_Choice",
                                Prompt = new ActivityTemplate("[EnterYourChoice]"),
                                Choices = new List<Choice>()
                                {
                                    new Choice("Confirm The First One"),
                                    new Choice("Confirm The Second One"),
                                    new Choice("Confirm The Third One"),
                                    new Choice("Next Page"),
                                    new Choice("Previous Page")
                                },
                                Style = ListStyle.SuggestedAction
                            },
                            new SwitchCondition()
                            {
                                Condition = "turn.CreateCalendarEntry_Choice",
                                Cases = new List<Case>()
                                {
                                    new Case("Confirm The First One", new List<IDialog>()
                                        {
                                            new IfCondition()
                                            {
                                                Condition = "dialog.matchedEmails[user.CreateCalendarEntry_pageIndex * 3] != null",
                                                Steps = new List<IDialog>()
                                                {
                                                    new SetProperty()
                                                    {
                                                        Property = "dialog.finalContact",
                                                        Value = "dialog.matchedEmails[user.CreateCalendarEntry_pageIndex * 3]"
                                                    }
                                                },
                                                ElseSteps = new List<IDialog>(){
                                                    new SendActivity("[viewEmptyEntry]"),
                                                    new RepeatDialog()
                                                }
                                            }
                                        }),
                                    new Case("Confirm The Second One", new List<IDialog>()
                                        {
                                            new IfCondition()
                                            {
                                                Condition = "dialog.matchedEmails[user.CreateCalendarEntry_pageIndex * 3 + 1] != null",
                                                Steps = new List<IDialog>()
                                                {
                                                    new SetProperty()
                                                    {
                                                        Property = "dialog.finalContact",
                                                        Value = "dialog.matchedEmails[user.CreateCalendarEntry_pageIndex * 3 + 1]"
                                                    }
                                                },
                                                ElseSteps = new List<IDialog>(){
                                                    new SendActivity("[viewEmptyEntry]"),
                                                    new RepeatDialog()
                                                }
                                            }
                                        }),
                                    new Case("Confirm The Third One", new List<IDialog>()
                                        {
                                            new IfCondition()
                                            {
                                                Condition = "dialog.matchedEmails[user.CreateCalendarEntry_pageIndex * 3 + 2] != null",
                                                Steps = new List<IDialog>()
                                                {
                                                    new SetProperty()
                                                    {
                                                        Property = "dialog.finalContact",
                                                        Value = "dialog.matchedEmails[user.CreateCalendarEntry_pageIndex * 3  + 2]"
                                                    }
                                                },
                                                ElseSteps = new List<IDialog>(){
                                                    new SendActivity("[viewEmptyEntry]"),
                                                    new RepeatDialog()
                                                }
                                            }
                                        }),
                                    new Case("Next Page", new List<IDialog>()
                                        {
                                            new SendActivity("Next page"),
                                            new SendActivity("{user.CreateCalendarEntry_pageIndex}"),
                                            new IfCondition()
                                            {
                                                Condition = "user.CreateCalendarEntry_pageIndex * 3 + 3 < count(dialog.matchedEmails) ",
                                                Steps = new List<IDialog>()
                                                {
                                                    new SetProperty()
                                                    {
                                                       Property = "user.CreateCalendarEntry_pageIndex",
                                                        Value = "user.CreateCalendarEntry_pageIndex + 1"
                                                    },
                                                    new RepeatDialog()
                                                },
                                                ElseSteps = new List<IDialog>()
                                                {
                                                    new SendActivity("This is already the last page!"),
                                                    new RepeatDialog()
                                                }
                                            }
                                        }),
                                    new Case("Previous Page", new List<IDialog>()
                                        {
                                            new IfCondition()
                                            {
                                                Condition = " 0 < user.CreateCalendarEntry_pageIndex",
                                                Steps = new List<IDialog>()
                                                {
                                                    new SetProperty()
                                                    {
                                                        Property = "user.CreateCalendarEntry_pageIndex",
                                                        Value = "user.CreateCalendarEntry_pageIndex - 1"
                                                    },
                                                    new RepeatDialog()
                                                },
                                                ElseSteps = new List<IDialog>()
                                                {
                                                    new SendActivity("This is already the first page!"),
                                                    new RepeatDialog()
                                                }
                                            }
                                        }),
                                },
                                Default = new List<IDialog>()
                                {
                                    new SendActivity("Sorry, I don't know what you mean!"),
                                    new EndDialog()
                                }
                            }
                        },
                        ElseSteps = new List<IDialog>()
                        {
                            new SendActivity("Sorry, We could not find any matches in your contacts. We will use the email address you just entered."),
                            new SetProperty(){
                                Property = "dialog.finalContact",
                                Value = "user.CreateCalendarEntry_PersonName"
                            }
                        }
                    },
                     // new saveproperty not usable TODO
                    new TextInput()
                    {
                        Property = "dialog.CreateCalendarEntry_Subject",
                        Prompt = new ActivityTemplate("[GetSubject]")
                    },
                    // new DateTimeInput(){ not usable TODO
                    new TextInput()
                    {
                        Property = "dialog.CreateCalendarEntry_FromTime",
                        Prompt = new ActivityTemplate("[GetFromTime]")
                    },
                    // new DateTimeInput()
                    new TextInput()
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
                    new DeleteProperty
                    {
                        Property = "turn.CreateCalendarEntry_ConfirmChoice"
                    },
                    new ChoiceInput()
                    {
                         Property = "turn.CreateCalendarEntry_ConfirmChoice",
                         Prompt = new ActivityTemplate("Is Your Information Correct?"),
                         Choices = new List<Choice>()
                         {
                            new Choice("Yes, Create It!"),
                            new Choice("No, Forget It!")
                         },
                        Style = ListStyle.SuggestedAction
                    },
                    new SetProperty()
                    {
                        Property = "user.CreateSuccess",
                        Value = "true"
                    },
                    // to post our latest update to our calendar
                    new SwitchCondition()
                    {
                        Condition = "turn.CreateCalendarEntry_ConfirmChoice",
                        Cases = new List<Case>(){
                            new Case("Yes, Create It!", new List<IDialog>()
                            {
                                new HttpRequest()
                                {
                                    Property = "dialog.createResponse",
                                    Method = HttpRequest.HttpMethod.POST,
                                    Url = "https://graph.microsoft.com/v1.0/me/events",
                                    Headers =  new Dictionary<string, string>(){
                                        ["Authorization"] = "Bearer {user.token.Token}",
                                    },
                                    Body = JObject.Parse(@"{
                                        'subject': '{dialog.CreateCalendarEntry_Subject}',
                                        'attendees': [
                                            {
                                            'emailAddress': {
                                                'address': '{dialog.finalContact}'
                                            }
                                            }
                                        ],
                                        'location': {
                                            'displayName': '{dialog.CreateCalendarEntry_Location}',
                                        },
                                        'start': {
                                            'dateTime': '{dialog.CreateCalendarEntry_FromTime}',
                                            'timeZone': 'UTC'
                                        },
                                        'end': {
                                            'dateTime': '{dialog.CreateCalendarEntry_ToTime}',
                                            'timeZone': 'UTC'
                                        }
                                    }")
                                }
                            }),
                            new Case("No, Forget It!", new List<IDialog>()
                            {
                                new SendActivity("Sure! let start over!"),
                                new RepeatDialog()
                            })
                        },
                        Default = new List<IDialog>()
                        {
                            new SendActivity("Sorry, I don't know what you mean!"),
                            new EndDialog()
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
                            new SendActivity("[CreateCalendarEntryFailed]")
                        },
                    },
                    new SendActivity("[Welcome-Actions]"),
                    new EndDialog()
                },

                // note: every input will be detected through this layer first from root dialog
                // once matched. Otherwise, the input will be thrown to the uppser layer.
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
                                new SendActivity("[CancelCreateMeeting]"),
                                new EndDialog()
                        }
                    }
                }
            };
            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(createCalendarEntry);

            // The initial child Dialog to run.
            InitialDialogId = "create";
        }

        private static IRecognizer CreateRecognizer()
        {
            return new RegexRecognizer()
            {
                Intents = new Dictionary<string, string>()
                {
                    { "Help", "(?i)help" },
                    {  "Cancel", "(?i)cancel|never mind"}
                }
            };
        }
    }
}
