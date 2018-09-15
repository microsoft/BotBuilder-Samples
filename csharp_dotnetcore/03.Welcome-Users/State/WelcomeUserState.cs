// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace WelcomeUser.State
{
    /// <summary>
    /// Represent a state object for a given user in a conversation.
    /// The state object is used to keep track of various state related to a user in a conversation.
    /// In this example, we are tracking if the bot has replied to customer first interaction.
    /// </summary>
    public class WelcomeUserState
    {
        public bool DidBotWelcomedUser { get; set; } = false;
    }
}
