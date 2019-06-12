using System.Collections.Generic;
using System.Threading;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Expressions.Parser;

namespace Microsoft.BotBuilderSamples
{
    public class ShowNextCalendar : ComponentDialog
    {
        public ShowNextCalendar()
            : base(nameof(ShowNextCalendar))
        {

        }
    }
}
