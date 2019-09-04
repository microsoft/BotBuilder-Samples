// <copyright file="EchoStateAccessor.cs" company="Microsoft">
// Licensed under the MIT License.
// </copyright>

namespace Microsoft.Bot.Builder.Teams.TeamEchoBot
{
    using System;
    using Microsoft.Bot.Builder.Teams.StateStorage;

    /// <summary>
    /// Echo state accessor.
    /// </summary>
    public class EchoStateAccessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EchoStateAccessor"/> class.
        /// Contains the <see cref="ConversationState"/> and associated <see cref="IStatePropertyAccessor{T}"/>.
        /// </summary>
        /// <param name="conversationState">The state object that stores the counter.</param>
        public EchoStateAccessor(TeamSpecificConversationState conversationState)
        {
            this.ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
        }

        /// <summary>
        /// Gets the <see cref="IStatePropertyAccessor{T}"/> name used for the <see cref="CounterState"/> accessor.
        /// </summary>
        /// <remarks>Accessors require a unique name.</remarks>
        /// <value>The accessor name for the counter accessor.</value>
        public static string CounterStateName { get; } = $"{nameof(EchoStateAccessor)}.EchoState";

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for EchoState.
        /// </summary>
        /// <value>
        /// The accessor stores the turn count for the conversation.
        /// </value>
        public IStatePropertyAccessor<EchoState> CounterState { get; set; }

        /// <summary>
        /// Gets the <see cref="TeamSpecificConversationState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="TeamSpecificConversationState"/> object.</value>
        public TeamSpecificConversationState ConversationState { get; }
    }
}
