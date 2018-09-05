// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WelcomeUser.State
{
    /// <summary>
    /// Represent state per user in a conversation.
    /// </summary>
    public class WelcomeUserState
    {
        public bool DidBotWelcomedUser { get; set; }
    }
}
