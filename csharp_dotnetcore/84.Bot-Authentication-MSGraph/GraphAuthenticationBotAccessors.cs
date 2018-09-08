// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This is a helper class to support the state accessors for the bot.
    /// </summary>
    public class GraphAuthenticationBotAccessors
    {
        // The name of the dialog state.
        public static readonly string DialogStateName = $"{nameof(GraphAuthenticationBotAccessors)}.DialogState";

        // The name of the command state.
        public static readonly string CommandStateName = $"{nameof(GraphAuthenticationBotAccessors)}.CommandState";

        /// <summary>
        /// Gets or Sets the DialogState accessor value.
        /// </summary>
        /// <value>
        /// A <see cref="DialogState"/> representing the state of the conversation.
        /// </value>
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }

        /// <summary>
        /// Gets or Sets the DialogState accessor value.
        /// </summary>
        /// <value>
        /// A string representing the command a user would like to execute.
        /// </value>
        public IStatePropertyAccessor<string> CommandState { get; set; }
    }
}
