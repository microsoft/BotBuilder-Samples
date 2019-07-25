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
/// This dialog will find all matched email given the contact name. It may be repeatedly call itself to add more contact.
/// Finally, this dialog will fulfill a desired contact list.
/// </summary>
namespace Microsoft.BotBuilderSamples
{
    public class AddContactDialog : ComponentDialog
    {
        private static IConfiguration Configuration;
        public AddContactDialog(IConfiguration configuration)
            : base(nameof(AddContactDialog))
        {
            Configuration = configuration;
            var addContactDialog = new AdaptiveDialog("addContact")
            {
                Recognizer = CreateRecognizer(),
                Generator = new ResourceMultiLanguageGenerator("AddContactDialog.lg"),
                Steps = new List<IDialog>()
                {
                    new TextInput()
                    {
                        Property = "user.CreateCalendarEntry_PersonName",
                        Prompt = new ActivityTemplate("[GetPersonName]")
                    },
                    new HttpRequest()
                    {
                        Property = "dialog.AddContactDialog_UserAll",
                        Method = HttpRequest.HttpMethod.GET,
                        Url = "https://graph.microsoft.com/v1.0/me/contacts",
                        Headers = new Dictionary<string, string>()
                        {
                            ["Authorization"] = "Bearer {user.token.Token}",
                        }
                    },
                    new IfCondition()
                    {
                        Condition = "dialog.AddContactDialog_UserAll.value != null && count(dialog.AddContactDialog_UserAll.value) > 0",
                        Steps = new List<IDialog>(){
                            new Foreach()
                            {
                                ListProperty = "dialog.AddContactDialog_UserAll.value",
                                Steps = new List<IDialog>()
                                {
                                    new IfCondition()
                                    {
                                        Condition = "contains(dialog.AddContactDialog_UserAll.value[dialog.index].displayName, user.CreateCalendarEntry_PersonName) == true ||" +
                                            "contains(dialog.AddContactDialog_UserAll.value[dialog.index].emailAddresses[0].address, user.CreateCalendarEntry_PersonName) == true",
                                        Steps = new List<IDialog>()
                                        {
                                            new EditArray()
                                            {
                                                ArrayProperty = "dialog.matchedEmails",
                                                ChangeType = EditArray.ArrayChangeType.Push,
                                                Value = "dialog.AddContactDialog_UserAll.value[dialog.index].emailAddresses[0].address"
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
                            new ChoiceInput(){
                                Property = "turn.CreateCalendarEntry_Choice",
                                Prompt = new ActivityTemplate("[EnterYourChoice]"),
                                Choices = new List<Choice>()
                                {
                                    new Choice("Confirm The First One"),
                                    new Choice("Confirm The Second One"),
                                    new Choice("Confirm The Third One")
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
                                                        Property = "user.finalContact",
                                                        Value = "dialog.matchedEmails[user.CreateCalendarEntry_pageIndex * 3]",
                                                        //Value = "concat(user.finalContact, '{\"emailAddress\":{ \"address\":\"', dialog.matchedEmails[user.CreateCalendarEntry_pageIndex * 3], '\"}},')"
                                                    },
                                                },
                                                ElseSteps = new List<IDialog>()
                                                {
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
                                                        Property = "user.finalContact",
                                                         Value = "dialog.matchedEmails[user.CreateCalendarEntry_pageIndex * 3 + 1]",
                                                         //Value = "concat(user.finalContact, '{\"emailAddress\":{ \"address\":\"', dialog.matchedEmails[user.CreateCalendarEntry_pageIndex * 3 + 1], '\"}},')"
                                                    }
                                                },
                                                ElseSteps = new List<IDialog>()
                                                {
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
                                                        Property = "user.finalContact",
                                                        Value = "dialog.matchedEmails[user.CreateCalendarEntry_pageIndex * 3 + 2]",
                                                        //Value = "concat(user.finalContact, '{\"emailAddress\":{ \"address\":\"', dialog.matchedEmails[user.CreateCalendarEntry_pageIndex * 3 + 2], '\"}},')"

                                                    }
                                                },
                                                ElseSteps = new List<IDialog>()
                                                {
                                                    new SendActivity("[viewEmptyEntry]"),
                                                    new RepeatDialog()
                                                }
                                            }
                                        })
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
                            new SetProperty()
                            {
                                Property = "user.finalContact",
                                Value = "user.CreateCalendarEntry_PersonName",
                                //Value = "concat(user.finalContact, '{\"emailAddress\":{ \"address\":\"', user.CreateCalendarEntry_PersonName, '\"}},')"
                            }
                        },
                    },
                    // For multiple contacts use only 
                    //new IfCondition()
                    //{
                    //    Condition = "user.repeatFlag == true",
                    //    Steps = new List<IDialog>(){
                    //        new ConfirmInput()
                    //        {
                    //            Property = "turn.AddContactDialog_ConfirmChoice",
                    //            Prompt = new ActivityTemplate("Do you want to add another contact?"),
                    //            InvalidPrompt = new ActivityTemplate("Please Say Yes/No."),
                    //        },
                    //        new IfCondition()
                    //        {
                    //            Condition = "turn.AddContactDialog_ConfirmChoice",
                    //            Steps = new List<IDialog>()
                    //            {
                    //                new DeleteProperty()
                    //                {
                    //                    Property = "user.CreateCalendarEntry_PersonName"
                    //                },
                    //                new RepeatDialog()
                    //            },
                    //            ElseSteps = new List<IDialog>(){
                    //                new SendActivity("{user.finalContact}"),
                    //                new EndDialog()
                    //            }
                    //        }
                    //    }
                    //}
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
                                new SendActivity("[CancelCreateMeeting]"),
                                new EndDialog()
                        }
                    },
                    new IntentRule("ShowPrevious")
                    {
                        Steps = new List<IDialog>()
                        {
                        new IfCondition()
                        {
                            Condition = " 0 < user.AddContactDialog_pageIndex",
                            Steps = new List<IDialog>()
                            {
                                new SetProperty()
                                {
                                    Property = "user.AddContactDialog_pageIndex",
                                    Value = "user.AddContactDialog_pageIndex - 1"
                                },
                                new RepeatDialog()
                            },
                            ElseSteps = new List<IDialog>()
                            {
                                new SendActivity("This is already the first page!"),
                                new RepeatDialog()
                            }
                        }
                        }
                    },
                    new IntentRule("ShowNext")
                    {
                        Steps = new List<IDialog>(){
                            new IfCondition()
                            {
                                Condition = "user.AddContactDialog_pageIndex * 3 + 3 < count(dialog.matchedEmails) ",
                                Steps = new List<IDialog>()
                                {
                                    new SetProperty()
                                    {
                                        Property = "user.AddContactDialog_pageIndex",
                                        Value = "user.AddContactDialog_pageIndex + 1"
                                    },
                                    new RepeatDialog()
                                },
                                ElseSteps = new List<IDialog>()
                                {
                                    new SendActivity("This is already the last page!"),
                                    new RepeatDialog()
                                }
                            }
                        }
                    }
                }
            };
            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(addContactDialog);
            // The initial child Dialog to run.
            InitialDialogId = "addContact";
        }
        public static IRecognizer CreateRecognizer()
        {
            if (string.IsNullOrEmpty(Configuration["LuisAppIdGeneral"]) || string.IsNullOrEmpty(Configuration["LuisAPIKeyGeneral"]) || string.IsNullOrEmpty(Configuration["LuisAPIHostNameGeneral"]))
            {
                throw new Exception("Your LUIS application is not configured. Please see README.MD to set up a LUIS application.");
            }
            return new LuisRecognizer(new LuisApplication()
            {
                Endpoint = Configuration["LuisAPIHostNameGeneral"],
                EndpointKey = Configuration["LuisAPIKeyGeneral"],
                ApplicationId = Configuration["LuisAppIdGeneral"]
            });
        }
    }
}
