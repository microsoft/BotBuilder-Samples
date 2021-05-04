// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.welcomeuser;

/**
 * This is the welcome state for this sample.
 *
 * Stores User Welcome state for the conversation.
 * Stored in "com.microsoft.bot.builder.ConversationState" and
 * backed by "com.microsoft.bot.builder.MemoryStorage".
 * 
 * <p>
 * NOTE: Standard Java getters/setters must be used for properties.
 * Alternatively, the Jackson JSON annotations could be used instead. If any
 * methods start with "get" but aren't a property, the Jackson JSON 'JsonIgnore'
 * annotation must be used.
 * </p>
 *
 * @see WelcomeUserBot
 */
public class WelcomeUserState {
    private boolean didBotWelcomeUser = false;

    // Gets whether the user has been welcomed in the conversation.
    public boolean getDidBotWelcomeUser() {
        return didBotWelcomeUser;
    }

    // Sets whether the user has been welcomed in the conversation.
    public void setDidBotWelcomeUser(boolean withDidWelcomUser) {
        didBotWelcomeUser = withDidWelcomUser;
    }
}
