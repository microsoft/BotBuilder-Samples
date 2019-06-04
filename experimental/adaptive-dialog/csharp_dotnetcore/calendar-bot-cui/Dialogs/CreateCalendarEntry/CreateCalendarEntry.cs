using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions.Parser;



/// <summary>
/// Known bug
/// 1. cannot go to next step // must say the subject is what?
/// 2. what if user wants to pass some information // use other code
/// </summary>
namespace Microsoft.BotBuilderSamples
{
    public class CreateCalendarEntry : ComponentDialog
    {
        public CreateCalendarEntry()
            : base(nameof(CreateCalendarEntry))
        {
            var CreateCalendarEntry = new AdaptiveDialog(nameof(AdaptiveDialog));
            CreateCalendarEntry.Recognizer = CreateRecognizer();
            CreateCalendarEntry.Steps = new List<IDialog>()
            {
                new EditArray(){
                    ChangeType = EditArray.ArrayChangeType.Clear,
                    ArrayProperty = "user.Entries"
                },
                new SaveEntity("@Subject[0]", "turn.Subject"),
                new SaveEntity("@FromDate[0]", "turn.FromDate"),
                new SaveEntity("@FromTime[0]", "turn.FromTime"),
                new SaveEntity("@DestinationCalendar[0]","turn.DestinationCalendar"),
                new SaveEntity("@ToDate[0]","turn.ToDate"),
                new SaveEntity("@ToTime[0]","turn.ToTime"),
                new SaveEntity("@Location[0]","turn.Location"),
                new SaveEntity("@Duration[0]","turn.Duration"),

                new TextInput()
                {
                    Property = "turn.Subject",
                    Prompt = new ActivityTemplate("[GetSubject]")
                },
                new SendActivity("{turn.Subject}"),
                //new TextInput()
                //{
                //    Property = "turn.FromDate",
                //    Prompt = new ActivityTemplate("[GetFromDate]")
                //},
                //new TextInput()
                //{
                //    Property = "turn.FromTime",
                //    Prompt = new ActivityTemplate("[GetFromTime]")
                //},
                ////new TextInput()
                ////{
                ////    Property = "turn.DestinationCalendar",
                ////    Prompt = new ActivityTemplate("[GetDestinationCalendar]")
                ////},
                ////new TextInput()
                ////{
                ////    Property = "turn.ToDate",
                ////    Prompt = new ActivityTemplate("[GetToDate]")
                ////},
                //// uncomment to prompt user to enter more information
                //new TextInput()
                //{
                //    Property = "turn.ToTime",
                //    Prompt = new ActivityTemplate("[GetToTime]")
                //},
                //new TextInput()
                //{
                //    Property = "turn.Location",
                //    Prompt = new ActivityTemplate("[GetLocation]")
                //},
                //new TextInput()
                //{
                //    Property = "turn.MeetingRoom",
                //    Prompt = new ActivityTemplate("[GetMeetingRoom]")
                //},
                //new IfCondition{
                //    Condition = new ExpressionEngine().Parse("turn.FromDate != null && turn.ToDate == null"),// TODO BUG might be here
                //    Steps = new List<IDialog>()
                //    {
                //        new TextInput()
                //        {
                //            Property = "turn.Duration",
                //            Prompt = new ActivityTemplate("[GetDuration]")
                //        }
                //  }
                //},
                //new EditArray()
                //{
                //    ItemProperty = "turn.Subject",
                //    ArrayProperty = "user.Entries",
                //    ChangeType = EditArray.ArrayChangeType.Push
                //},
                new SendActivity("[CreateCalendarEntryReadBack]")
            };
            CreateCalendarEntry.Rules = new List<IRule>()
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
                                new SendActivity("[HelpPrefix], let's get right back to scheduling a meeting.") // TODO add this to lg file
                            }
                        }
                    }
                },
                new IntentRule("CheckMessages")
                {
                    Steps = new List<IDialog>() { new BeginDialog(nameof(CreateCalendarEntry), null) },
                    Constraint = "turn.dialogEvent.value.intents.CheckMessages.score > 0.4"
                },
            };
            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(CreateCalendarEntry);
            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
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

