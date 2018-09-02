// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Using_Cards
{
    public class CardsBotAccessors
    {
        internal static string DialogStateName = $"{nameof(CardsBotAccessors)}.DialogState";

        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }
    }
}
