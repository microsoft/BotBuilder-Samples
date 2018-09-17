// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace BasicBot
{
    /// <summary>
    /// This class is created as a Singleton and passed into the IBot-derived constructor.
    ///  - See <see cref="BasicBot"/> constructor for how that is injected.
    ///  - See the Startup.cs file for more details on creating the Singleton that gets
    ///    injected into the constructor.
    /// </summary>
    public class BasicBotAccessors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicBotAccessors"/> class.
        /// Contains the <see cref="ConversationState"/> and associated <see cref="IStatePropertyAccessor{T}"/>.
        /// </summary>
        /// <param name="userState">The state object that stores the greeting state (name/city).</param>
        /// <param name="conversationState">The state object that stores the dialog state.</param>
        public BasicBotAccessors(UserState userState, ConversationState conversationState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }

        /// <summary>
        /// Gets the <see cref="IStatePropertyAccessor{T}"/> name used for the <see cref="CounterState"/> accessor.
        /// </summary>
        /// <remarks>Accessors require a unique name.</remarks>
        /// <value>The accessor name for the accessor.</value>
        public static string GreetingStateName { get; } = $"{nameof(BasicBotAccessors)}.GreetingState";

        /// <summary>
        /// Gets the <see cref="IStatePropertyAccessor{T}"/> name used for the <see cref="DialogState"/> accessor.
        /// </summary>
        /// <remarks>Accessors require a unique name.</remarks>
        /// <value>The accessor name for the <see cref="DialogStateProperty"/>accessor.</value>
        public static string DialogStateName { get; } = $"{nameof(BasicBotAccessors)}.DialogState";

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for <see cref="GreetingState"/>.
        /// </summary>
        /// <value>
        /// The accessor stores the dialog name and city for the user.
        /// </value>
        public IStatePropertyAccessor<GreetingState> GreetingStateProperty { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for <see cref="DialogState"/>.
        /// </summary>
        /// <value>
        /// The accessor stores the dialog name and city for the user.
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
