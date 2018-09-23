// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// The accessors we will be using in the bot logic. Having a class like this just allows them to be easily
    /// handed to the IBot instance through the dependency injection.
    /// </summary>
    public class MultiLingualBotAccessors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiLingualBotAccessors"/> class.
        /// Contains the <see cref="ConversationState"/> and associated <see cref="IStatePropertyAccessor{T}"/>.
        /// </summary>
        /// <param name="conversationState">The state object that stores the dialog state.</param>
        /// <param name="userState">The state object that stores the user state.</param>
        public MultiLingualBotAccessors(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }

        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }

        /// <summary>
        /// Gets or sets user language preference to drive translation.
        /// </summary>
        /// <value>
        /// User language preference.
        /// </value>
        public IStatePropertyAccessor<string> LanguagePreference { get; set; }

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
