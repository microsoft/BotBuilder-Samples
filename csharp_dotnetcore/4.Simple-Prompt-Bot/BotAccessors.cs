// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Simple_Prompt_Bot
{
    public class BotAccessors
    {
        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }
    }
}
