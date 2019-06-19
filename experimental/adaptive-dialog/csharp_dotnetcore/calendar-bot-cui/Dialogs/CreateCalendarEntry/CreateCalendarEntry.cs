using System;
using System.Collections.Generic;
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
                    /*new SaveEntity("@Subject[0]", "dialog.createCalendarEntry.Subject"),
                    new SaveEntity("@FromDate[0]", "dialog.createCalendarEntry.FromDate"),
                    new SaveEntity("@FromTime[0]", "dialog.createCalendarEntry.FromTime"),
                    new SaveEntity("@DestinationCalendar[0]","dialog.createCalendarEntry.DestinationCalendar"),
                    new SaveEntity("@ToDate[0]","dialog.createCalendarEntry.ToDate"),
                    new SaveEntity("@ToTime[0]","dialog.createCalendarEntry.ToTime"),
                    new SaveEntity("@Location[0]","dialog.createCalendarEntry.Location"),
                    new SaveEntity("@Duration[0]","dialog.createCalendarEntry.Duration"),*/


                    // TODO add personName for user to input
                    // turn will disappear
                    // save to the user state
                    // every input will be detected through LUIS first from root dialog
                    // once matched, the procedure will add another layer of dialog
                    // not matched. will skip the root dialog, and taken as input
                    new TextInput()
                    {
                        Property = "dialog.createCalendarEntry.Subject",
                        Prompt = new ActivityTemplate("[GetSubject]")
                    },

                    //new TextInput()
                    //{
                    //    Property = "dialog.createCalendarEntry.FromTime",
                    //    Prompt = new ActivityTemplate("[GetFromTime]")
                    //},
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
                    //    Property = "dialog.createCalendarEntry.ToTime",
                    //    Prompt = new ActivityTemplate("[GetToTime]")
                    //},
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

                    new SetProperty()
                    {
                        Property = "user.focusEntry",
                        Value = new ExpressionEngine().Parse("{dialog.createCalendarEntry.Subject}")
                    },
                    new SendActivity("Focus Completed"),
                    new SendActivity("{user.focusEntry}"),

                    new EditArray()
                    {
                        Value = new ExpressionEngine().Parse("dialog.createCalendarEntry.Subject"),
                        ArrayProperty = "user.Entries",
                        ChangeType = EditArray.ArrayChangeType.Push
                    },
                   
                    new SendActivity("[CreateCalendarEntryReadBack]"),
                    new EndDialog()
                }
            };

            // rules can be overrided
            // if found, do steps here
            // if not found, back to the top level
            createCalendarEntry.Rules = new List<IRule>()
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
            };
            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(createCalendarEntry);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }

        private Expression Expression(string v)
        {
            throw new NotImplementedException();
        }

        private static IRecognizer CreateRecognizer()//TODO not sure whether it should be changed
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

