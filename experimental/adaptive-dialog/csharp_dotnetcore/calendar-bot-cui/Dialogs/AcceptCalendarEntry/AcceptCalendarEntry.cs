using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions;
using Microsoft.Bot.Builder.Expressions.Parser;
using Microsoft.Bot.Builder.LanguageGeneration;
using Newtonsoft.Json.Linq;

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

                    new HttpRequest(){
                        Url = "https://graph.microsoft.com/v1.0/me/calendarview?startdatetime={utcNow()}&enddatetime={addDays(utcNow(), 1)}",
                        Method = HttpRequest.HttpMethod.GET,
                        Headers =  new Dictionary<string, string>()
                        {
                            ["Authorization"] = "Bearer {dialog.token.Token}",
                        },
                        Property = "dialog.acceptCalendarEntry_graphAll"
                    },

                    // Handle case where there are no items in todo list
                    new IfCondition()
                    {
                        Condition = new ExpressionEngine().Parse("dialog.acceptCalendarEntry_graphAll == null || count(dialog.acceptCalendarEntry_graphAll) <= 0"),
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[AccpetEmptyList]"),
                            new SendActivity("[Welcome-Actions]"),
                            new EndDialog()
                        }
                    },

                    // new SaveEntity("@Subject[0]", "dialog.deleteCalendarEntry_entrySubject"),                    
                    // new CodeStep(GetToDoTitleToDelete),
                   
                    new BeginDialog(nameof(FindCalendarEntry)),
                    new TextInput()
                    {
                        Property = "dialog.acceptCalendarEntry_entrySubject",
                        Prompt = new ActivityTemplate("[GetEntryTitleToAccept]"),
                    },

                    //new InitProperty()
                    //{
                    //    Property = "dialog.afterAccepted",
                    //    Type = "array"
                    //},

                    //new SendActivity("{dialog.acceptCalendarEntry_graphAll.value}"),

                    new Foreach()
                    {
                        ListProperty = new ExpressionEngine().Parse("dialog.acceptCalendarEntry_graphAll.value"),
                        Steps = new List<IDialog>()
                        {
                            new IfCondition()
                            {
                                Condition = new ExpressionEngine().Parse("dialog.acceptCalendarEntry_graphAll.value[dialog.index].subject == dialog.acceptCalendarEntry_entrySubject"),
                                Steps = new List<IDialog>(){
                                    //new SetProperty(){
                                    //    Property = "dialog.temp",
                                    //    Value = new ExpressionEngine().Parse("user.Entries[dialog.index]")                                        
                                    //},
                                    //new SetProperty(){
                                    //    Property = "dialog.temp.accept",
                                    //    Value = new ExpressionEngine().Parse("'accepted'")
                                    //},
                                    //new EditArray(){
                                    //    Value = new ExpressionEngine().Parse("dialog.temp"),
                                    //    ArrayProperty = "dialog.afterAccepted",
                                    //    ChangeType = EditArray.ArrayChangeType.Push
                                    //},
                                    new IfCondition()
                                    {
                                        Condition = new ExpressionEngine().Parse("dialog.acceptCalendarEntry_graphAll.value[dialog.index].isOrganizer != true"),
                                        Steps = new List<IDialog>(){
                                            new HttpRequest(){// TODO bug exsits below
                                                Property = "user.acceptResponse",
                                                Method = HttpRequest.HttpMethod.POST,
                                                Url = "https://graph.microsoft.com/v1.0/me/events/{dialog.acceptCalendarEntry_graphAll.value[dialog.index].id}/accept",
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
                                //ElseSteps = new List<IDialog>(){
                                //    new EditArray(){
                                //        Value = new ExpressionEngine().Parse("user.Entries[dialog.index]"),
                                //        ArrayProperty = "dialog.afterAccepted",
                                //        ChangeType = EditArray.ArrayChangeType.Push
                                //    }
                                //}
                            }
                        }
                    },

                    //new SetProperty(){
                    //    Property = "user.Entries",
                    //    Value = new ExpressionEngine().Parse("dialog.afterAccepted")
                    //},

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
