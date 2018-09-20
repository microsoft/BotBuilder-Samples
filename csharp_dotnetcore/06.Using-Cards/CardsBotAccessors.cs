// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
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
        /// <remarks>Accessors require a unique name.</remarks>
        /// <value>The accessor name for the dialog state accessor.</value>
        public static readonly string DialogStateName = $"{nameof(CardsBotAccessors)}.DialogState";

        /// <summary>
        /// Gets or sets the <see cref="ConversationState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="ConversationState"/> object.</value>
        public IStatePropertyAccessor<DialogState> ConversationState { get; set; }
    }
}
