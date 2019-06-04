using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions.Parser;

namespace Microsoft.BotBuilderSamples
{
    public class FindCalendarEntry : ComponentDialog
    {
        public FindCalendarEntry()
            : base(nameof(FindCalendarEntry))
        {
            // Create instance of adaptive dialog. 
            var FindCalendarEntry = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
                Steps = new List<IDialog>()
                {
                    new IfCondition()
                    {
                        Condition = new ExpressionEngine().Parse("user.Entries != null && count(user.Entries) > 0"),
                        Steps = new List<IDialog>()
                        {
                            new SendActivity("[ViewEntries]"),
                            new EndDialog()
                        },
                        ElseSteps = new List<IDialog>
                        {
                            new SendActivity("[NoEntries]"),
                            new EndDialog()
                        }
                    }
                }
            };

            // Add named dialogs to the DialogSet. These names are saved in the dialog state.
            AddDialog(FindCalendarEntry);

            // The initial child Dialog to run.
            InitialDialogId = nameof(AdaptiveDialog);
        }
    }
}
