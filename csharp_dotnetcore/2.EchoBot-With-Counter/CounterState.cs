// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace EchoBotWithCounter
{
    /// <summary>
    /// Class for storing the counter state for the conversation.
    /// In this sample, this gets stored in <see cref="Microsoft.Bot.Builder.ConversationState"/> and is
    /// backed by <see cref="Microsoft.Bot.Builder.MemoryStorage"/>.
    /// </summary>
    public class CounterState
    {
        /// <summary>
        /// Gets or sets the number of messages the user has sent in the current conversation.
        /// </summary>
        /// <value>The number of turns in the conversation.</value>
        public int TurnCount { get; set; } = 0;
    }
}
