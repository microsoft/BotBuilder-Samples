using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    public class InterruptOptions : DialogOptions
    {
        public InterruptOptions()
        {
        }

        public Dictionary<string, object> Values { get; } = new Dictionary<string, object>();
    }
}
