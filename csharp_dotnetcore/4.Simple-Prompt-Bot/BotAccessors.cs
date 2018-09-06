// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This class allows the accessors defined in the Startup code to be handed to ever instance of our Bot.
    /// </summary>
    public class BotAccessors
    {
        // Conversation state is of type DialogState. Under the covers this is a serialized dialog stack.
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }
    }
}
