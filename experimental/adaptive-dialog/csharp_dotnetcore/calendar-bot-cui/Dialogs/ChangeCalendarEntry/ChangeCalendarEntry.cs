using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions.Parser;
using Microsoft.Bot.Builder.LanguageGeneration;

namespace Microsoft.BotBuilderSamples
{
    public class ChangeCalendarEntry : ComponentDialog
    {
        public ChangeCalendarEntry()
            : base(nameof(ChangeCalendarEntry))
        {
            var changeCalendarEntry = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Generator = new ResourceMultiLanguageGenerator("FindCalendarEntry.lg"),
                Steps = new List<IDialog>()
                {
                    new Foreach(){
                        ListProperty = new ExpressionEngine().Parse("user.Entries"), //DEBUG ONLY SHOW ACTIONS ONCE, SHOULD APPEAR THREE TIMES
                        Steps = new List<IDialog>(){
                            new SendActivity("Hello World")
                        }
                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(changeCalendarEntry);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }
    }
}
