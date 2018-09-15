// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
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
        /// Initializes a new instance of the <see cref="MessageRoutingBotAccessors"/> class.
        /// Contains the <see cref="ConversationState"/> and associated <see cref="IStatePropertyAccessor{T}"/>.
        /// </summary>
        /// <param name="conversationState">The state object that stores the dialog state.</param>
        /// <param name="userState">The state object that stores the user state.</param>
        public MessageRoutingBotAccessors(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }

        /// <summary>
        /// Gets or sets the <see cref="DialogState"/> properties for the conversation.
        /// </summary>
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }

        /// <summary>
        /// Gets the <see cref="ConversationState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="ConversationState"/> object.</value>
        public ConversationState ConversationState { get; }

        /// <summary>
        /// Gets the <see cref="UserState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="UserState"/> object.</value>
        public UserState UserState { get; }
    }
}
