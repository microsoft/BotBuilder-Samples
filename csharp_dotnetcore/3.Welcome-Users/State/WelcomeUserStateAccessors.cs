// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder;

namespace WelcomeUser.State
{
    /// <summary>
    /// This class holds a set of accessors (to specific properties) that the bot uses to access
    /// specific data. These are created as singleton via DI.
    /// </summary>
    public class WelcomeUserStateAccessors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomeUserStateAccessors"/> class.
        /// </summary>
        /// <param name="userState">A <see cref="UserState"/> object used to save sate.</param>
        public WelcomeUserStateAccessors(UserState userState)
        {
            UserState = userState;
        }

        public IStatePropertyAccessor<bool> DidBotWelcomedUser { get; set; }

        public UserState UserState { get; }
    }
}
