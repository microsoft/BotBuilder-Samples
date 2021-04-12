// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.statemanagement;

/**
 * This is the conversation data for this sample.
 *
 * <p>
 * NOTE: Standard Java getters/setters must be used for properties.
 * Alternatively, the Jackson JSON annotations could be used instead. If any
 * methods start with "get" but aren't a property, the Jackson JSON 'JsonIgnore'
 * annotation must be used.
 * </p>
 *
 * @see StateManagementBot
 */
public class ConversationData {
    private String timestamp;
    private String channelId;
    private boolean promptedUserForName;

    public String getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(String withTimestamp) {
        timestamp = withTimestamp;
    }

    public String getChannelId() {
        return channelId;
    }

    public void setChannelId(String withChannelId) {
        channelId = withChannelId;
    }

    public boolean getPromptedUserForName() {
        return promptedUserForName;
    }

    public void setPromptedUserForName(boolean withPromptedUserForName) {
        promptedUserForName = withPromptedUserForName;
    }
}
