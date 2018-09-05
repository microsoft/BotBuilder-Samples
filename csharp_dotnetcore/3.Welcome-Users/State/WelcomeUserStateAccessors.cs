// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace WelcomeUser.State
{
    /// <summary>
    /// This class holds set of accessors (to specific properties) that the bot uses to access
    /// specific data. These are created as singleton via DI.
    /// </summary>
    public class WelcomeUserStateAccessors
    {
        public IStatePropertyAccessor<bool> DidBotWelcomedUser { get; set; }
    }
}
