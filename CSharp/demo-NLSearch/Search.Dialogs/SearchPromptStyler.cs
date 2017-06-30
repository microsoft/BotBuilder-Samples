using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace Search.Dialogs
{
    [Serializable]
    class SearchPromptStyler: PromptStyler
    {
        private int _maxKeyboard;

        public SearchPromptStyler(int maxKeyboard = 11)
            : base(PromptStyle.Keyboard)
        {
            _maxKeyboard = maxKeyboard;
        }

        public override void Apply<T>(ref IMessageActivity message, string prompt, IReadOnlyList<T> options, IReadOnlyList<string> descriptions = null, string speak = null)
        {
            var style = this.PromptStyle;
            if (options != null && options.Count() > _maxKeyboard && style == PromptStyle.Keyboard)
            {
                style = PromptStyle.Auto;
            }
            PromptStyler.Apply<T>(ref message, prompt, options, style, descriptions, speak);
        }
    }
}
