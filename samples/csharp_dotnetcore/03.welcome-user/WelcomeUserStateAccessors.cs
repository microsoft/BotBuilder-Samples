// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Bot.Builder;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This class holds a set of accessors (to specific properties) that the bot uses to access
    /// specific data. These are created as singleton via DI.
    /// </summary>
    public class WelcomeUserStateAccessors
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomeUserStateAccessors"/> class.
        /// Contains the <see cref="UserState"/> and associated <see cref="IStatePropertyAccessor{T}"/>.
        /// </summary>
        /// <param name="userState">The state object that stores the counter.</param>
        public WelcomeUserStateAccessors(UserState userState)
        {
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }

        /// <summary>
        /// Gets the <see cref="IStatePropertyAccessor{T}"/> name used for the <see cref="BotBuilderSamples.WelcomeUserState"/> accessor.
        /// </summary>
        /// <remarks>Accessors require a unique name.</remarks>
        /// <value>The accessor name for the WelcomeUser state.</value>
        public static string WelcomeUserName { get; } = $"{nameof(WelcomeUserStateAccessors)}.WelcomeUserState";

        /// <summary>
        /// Gets or sets the <see cref="IStatePropertyAccessor{T}"/> for DidBotWelcome.
        /// </summary>
        /// <value>
        /// The accessor stores if the bot has welcomed the user or not.
        /// </value>
        public IStatePropertyAccessor<WelcomeUserState> WelcomeUserState { get; set; }

        /// <summary>
        /// Gets the <see cref="UserState"/> object for the conversation.
        /// </summary>
        /// <value>The <see cref="UserState"/> object.</value>
        public UserState UserState { get; }
    }
}
