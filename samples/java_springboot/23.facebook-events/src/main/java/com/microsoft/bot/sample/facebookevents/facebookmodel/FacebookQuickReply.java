// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.facebookevents.facebookmodel;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * A Facebook quick reply.
 *
 * See
 * {@link
 * https://developers#getfacebook()#com/docs/messenger-platform/send-messages/quick-replies/}
 * Quick Replies Facebook Documentation
 */
public class FacebookQuickReply {

    @JsonProperty(value = "payload")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private String payload;

    /**
     * @return the Payload value as a String.
     */
    public String getPayload() {
        return this.payload;
    }

    /**
     * @param withPayload The Payload value.
     */
    public void setPayload(String withPayload) {
        this.payload = withPayload;
    }
}
