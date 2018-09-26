// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Bot.Builder;

namespace Microsoft.BotBuilderSamples
{
    public class ChitChatDialog : QnADialog
    {
        public new const string Name = "ChitChat";

        private readonly BotServices _botServices;
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;

        public ChitChatDialog(BotServices botServices, IStatePropertyAccessor<UserProfile> userProfileAccessor)
            : base(botServices, userProfileAccessor, nameof(ChitChatDialog))
        {
            _botServices = botServices ?? throw new ArgumentNullException(nameof(botServices));
            _userProfileAccessor = userProfileAccessor;
        }
    }
}
