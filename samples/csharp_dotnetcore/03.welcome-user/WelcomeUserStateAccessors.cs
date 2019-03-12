// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;
namespace Microsoft.BotBuilderSamples
{
    // This class holds a set of accessors (to specific properties) that the bot uses to access
    // specific data. These are created as singleton via DI.
    public class WelcomeUserStateAccessors
    {
        // Initializes a new instance of the "WelcomeUserStateAccessors class.
        // Contains the "UserState" and associated "IStatePropertyAccessor{T}".     
        // "userState" is the state object that stores the user data.
        public WelcomeUserStateAccessors(UserState userState)
        {  
               UserState = userState ?? throw new ArgumentNullException(nameof(userState));     
        }

        // Gets the "IStatePropertyAccessor{T}" name used for the "BotBuilderSamples.WelcomeUserState" accessor.
        // Accessors require a unique name.
        // The value is the accessor name for the WelcomeUser state
        public static string WelcomeUserName { get; } = $"{nameof(WelcomeUserStateAccessors)}.WelcomeUserState";
     
        // Gets or sets the "IStatePropertyAccessor{T}" for DidBotWelcome.
        // The accessor stores if the bot has welcomed the user or not.
        public IStatePropertyAccessor<WelcomeUserState> WelcomeUserState { get; set; }

        // Gets the "UserState" object for the conversation.
        public UserState UserState { get; }
    }
}
