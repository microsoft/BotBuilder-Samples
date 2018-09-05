// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Using_Cards
{
    /// <summary>
    /// This is a helper class to support the state accessors for the bot.
    /// </summary>
    public class CardsBotAccessors
    {
        /// <summary>
        /// The name of the dialog state.
        /// </summary>
        public static readonly string DialogStateName = $"{nameof(CardsBotAccessors)}.DialogState";

        /// <summary>
        /// Gets or Sets the DialogState accessor value.
        /// </summary>
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }
    }
}
