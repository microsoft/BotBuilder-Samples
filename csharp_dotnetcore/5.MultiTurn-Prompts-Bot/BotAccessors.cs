// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace MultiTurn_Prompts_Bot
{
    public class BotAccessors
    {
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }

        public IStatePropertyAccessor<UserProfile> UserProfile { get; set; }
    }
}
