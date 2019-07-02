using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions.Parser;
using Microsoft.Bot.Builder.LanguageGeneration;

/// <summary>
/// This dialog will show all the calendar entries if they have the same email address
/// </summary>
namespace Microsoft.BotBuilderSamples
{
    public class FindCalendarWho : ComponentDialog
    {
        public FindCalendarWho()
            : base(nameof(FindCalendarWho))
        {
            // Create instance of adaptive dialog. 
            var findCalendarWho = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new ResourceMultiLanguageGenerator("FindCalendarWho.lg"),
                Steps = new List<IDialog>()
                {

                    new BeginDialog(nameof(OAuthPromptDialog)),
                    // get our calendar
                    new HttpRequest(){
                        Url = "https://graph.microsoft.com/v1.0/me/calendarview?startdatetime={utcNow()}&enddatetime={addDays(utcNow(), 1)}",
                        Method = HttpRequest.HttpMethod.GET,
                        Headers =  new Dictionary<string, string>()
                        {
                            ["Authorization"] = "Bearer {user.token.Token}",
                        },
                        Property = "dialog.FindCalendarWho_GraphAll"
                    },
                    // Handle case where there are no items in calendar
                    new IfCondition()
                    {
                        Condition = new ExpressionEngine().Parse("dialog.FindCalendarWho_GraphAll.value == null || count(dialog.FindCalendarWho_GraphAll.value) <= 0"),
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[ViewEmptyList]"),
                            new SendActivity("[Welcome-Actions]"),
                            new EndDialog()
                        }
                    },
                    // show our non-empty calendar
                    new BeginDialog(nameof(FindCalendarEntry)),
                    new TextInput()
                    {
                        Property = "dialog.findCalendarWho_entryName",
                        Prompt = new ActivityTemplate("[GetPersonName]"),
                    },
                    // this array will flag whether we find a match at all
                    new InitProperty()
                    {
                        Property = "dialog.findCalendarWho_found",
                        Type = "Array"
                    },
                    // to iterate all the entries to find all matches
                    new Foreach()
                    {
                        ListProperty = new ExpressionEngine().Parse("dialog.FindCalendarWho_GraphAll.value"),
                        Steps = new List<IDialog>()
                        {
                            new IfCondition()
                            {
                                Condition = new ExpressionEngine().Parse("dialog.FindCalendarWho_GraphAll.value[dialog.index].attendees[0].emailAddress.address == dialog.findCalendarWho_entryName"),
                                Steps = new List<IDialog>(){
                                    new EditArray(){
                                        ArrayProperty = "dialog.findCalendarWho_found",
                                        ChangeType = EditArray.ArrayChangeType.Push,
                                        Value = new ExpressionEngine().Parse("dialog.FindCalendarWho_GraphAll.value[dialog.index]")
                                    },
                                    new SendActivity("[entryTemplate]")
                                }
                            }
                        }
                    },
                    // no matches situation
                    new IfCondition()
                    {
                        Condition = new ExpressionEngine().Parse("dialog.findCalendarWho_found == null || count(dialog.findCalendarWho_found) <= 0"),
                        Steps = new List<IDialog>(){
                            new SendActivity("We could not find any entries, sorry"),
                            new SendActivity("[Welcome-Actions]"),
                            new EndDialog()
                        }

                    },
                    new SendActivity("[Welcome-Actions]"),
                    new EndDialog()
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(findCalendarWho);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }
    }
}
