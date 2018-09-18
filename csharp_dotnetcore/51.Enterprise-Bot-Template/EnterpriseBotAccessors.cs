// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace EnterpriseBot
{
    public class EnterpriseBotAccessors
    {
        public EnterpriseBotAccessors(UserState userState, ConversationState conversationState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }

        /// <summary>
        /// Gets the <see cref="IStatePropertyAccessor{T}"/> name used for the <see cref="DialogState"/> accessor.
        /// </summary>
        /// <remarks>Accessors require a unique name.</remarks>
        /// <value>The accessor name for the <see cref="DialogStateProperty"/>accessor.</value>
        public static string DialogStateName { get; } = $"{nameof(EnterpriseBotAccessors)}.DialogState";

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for <see cref="DialogState"/>.
        /// </summary>
        /// <value>
        /// The accessor stores the dialog state for the user.
        /// </value>
        public IStatePropertyAccessor<DialogState> DialogStateProperty { get; set; }

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
