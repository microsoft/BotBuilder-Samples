// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.facebookevents.facebookmodel;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Defines a Facebook recipient.
 */
public class FacebookRecipient {

    @JsonProperty(value = "id")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private String id;

    /**
     * The Facebook Id of the recipient.
     * @return the Id value as a String.
     */
    public String getId() {
        return this.id;
    }

    /**
     * The Facebook Id of the recipient.
     * @param withId The Id value.
     */
    public void setId(String withId) {
        this.id = withId;
    }
}
