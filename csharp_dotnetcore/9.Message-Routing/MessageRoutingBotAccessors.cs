// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace MessageRoutingBot
{
    /// <summary>
    /// Accessors for MessageRouting bot state.
    /// </summary>
    public class MessageRoutingBotAccessors
    {
        /// <summary>
        /// Access <see cref="DialogState"> properties for the conversation.
        /// </summary>
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }

        /// <summary>
        /// Gets <see cref="SemaphoreSlim"> for state synchronization.
        /// </summary>
        public SemaphoreSlim SemaphoreSlim { get; } = new SemaphoreSlim(1, 1);
    }
}
