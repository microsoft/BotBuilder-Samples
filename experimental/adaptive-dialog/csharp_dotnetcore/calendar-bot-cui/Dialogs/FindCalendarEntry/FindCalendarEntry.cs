using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions.Parser;
using Microsoft.Bot.Builder.LanguageGeneration;

/// <summary>
/// This dialog will show all the calendar entries.
/// </summary>
namespace Microsoft.BotBuilderSamples
{
    public class FindCalendarEntry : ComponentDialog
    {
        public FindCalendarEntry()
            : base(nameof(FindCalendarEntry))
        {
            // Create instance of adaptive dialog. 
            var findCalendarEntry = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new ResourceMultiLanguageGenerator("FindCalendarEntry.lg"),
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
                        Property = "dialog.FindCalendarEntry_GraphAll"
                    },
                    // to avoid shoing an empty calendar
                    new IfCondition()
                    {
                        Condition = new ExpressionEngine().Parse("dialog.FindCalendarEntry_GraphAll.value != null && count(dialog.FindCalendarEntry_GraphAll.value) > 0"),
                        Steps = new List<IDialog>()
                        {
                            //new SendActivity("[ViewEntries]"),
                            new Foreach(){
                                ListProperty = new ExpressionEngine().Parse("dialog.FindCalendarEntry_GraphAll.value"),
                                Steps = new List<IDialog>(){
                                    new SendActivity("[entryTemplate]")
                                }
                            },
                            new SendActivity("[Welcome-Actions]"),
                            new EndDialog()
                        },
                        ElseSteps = new List<IDialog>
                        {
                            new SendActivity("[NoEntries]"),
                            new SendActivity("[Welcome-Actions]"),
                            new EndDialog()
                        }
                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(findCalendarEntry);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }
    }
}
