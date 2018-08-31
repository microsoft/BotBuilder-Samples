using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Using_Cards
{
    public class CardsBotAccessors
    {
        public static string DialogStateName = $"{nameof(CardsBotAccessors)}.DialogState";
        public static string CommandStateName = $"{nameof(CardsBotAccessors)}.CommandState";

        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }
        public IStatePropertyAccessor<string> CommandState { get; set; }
    }
}

