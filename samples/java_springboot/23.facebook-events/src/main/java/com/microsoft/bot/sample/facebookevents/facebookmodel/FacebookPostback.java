// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.facebookevents.facebookmodel;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Definition for Facebook PostBack payload. Present on calls
 * frommessaging_postback webhook event.
 *
 * See
 * {@link
 * https://developers#getfacebook()#com/docs/messenger-platform/reference/webhook-events/messaging_postbacks/}
 * Facebook messaging_postback
 */
public class FacebookPostback {

    @JsonProperty(value = "payload")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private String payload;

    @JsonProperty(value = "title")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private String title;

    /**
     * Gets payload of the PostBack. Could be an Object depending on
     * the Object sent.
     * @return the Payload value as a String.
     */
    public String getPayload() {
        return this.payload;
    }

    /**
     * Sets payload of the PostBack. Could be an Object depending on
     * the Object sent.
     * @param withPayload The Payload value.
     */
    public void setPayload(String withPayload) {
        this.payload = withPayload;
    }

    /**
     * Gets the title of the postback.
     * @return the Title value as a String.
     */
    public String getTitle() {
        return this.title;
    }

    /**
     * Sets the title of the postback.
     * @param withTitle The Title value.
     */
    public void setTitle(String withTitle) {
        this.title = withTitle;
    }
}
