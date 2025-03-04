// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.

package com.microsoft.bot.sample.facebookevents;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Simple version of the payload received from the Facebook channel.
 */
public class FacebookPayload {

    @JsonProperty(value = "sender")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private FacebookSender sender;

    @JsonProperty(value = "recipient")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private FacebookRecipient recipient;

    @JsonProperty(value = "message")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private FacebookMessage message;

    @JsonProperty(value = "postback")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private FacebookPostback postBack;

    @JsonProperty(value = "optin")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private FacebookOptin optin;

    /**
     * Gets the sender of the message.
     * @return the Sender value as a FacebookSender.
     */
    public FacebookSender getSender() {
        return this.sender;
    }

    /**
     * Sets the sender of the message.
     * @param withSender The Sender value.
     */
    public void setSender(FacebookSender withSender) {
        this.sender = withSender;
    }

    /**
     * Gets the recipient of the message.
     * @return the Recipient value as a FacebookRecipient.
     */
    public FacebookRecipient getRecipient() {
        return this.recipient;
    }

    /**
     * Sets the recipient of the message.
     * @param withRecipient The Recipient value.
     */
    public void setRecipient(FacebookRecipient withRecipient) {
        this.recipient = withRecipient;
    }

    /**
     * Gets the message.
     * @return the Message value as a FacebookMessage.
     */
    public FacebookMessage getMessage() {
        return this.message;
    }

    /**
     * Sets the message.
     * @param withMessage The Message value.
     */
    public void setMessage(FacebookMessage withMessage) {
        this.message = withMessage;
    }

    /**
     * Gets the postback payload if available.
     * @return the PostBack value as a FacebookPostback.
     */
    public FacebookPostback getPostBack() {
        return this.postBack;
    }

    /**
     * Sets the postback payload if available.
     * @param withPostBack The PostBack value.
     */
    public void setPostBack(FacebookPostback withPostBack) {
        this.postBack = withPostBack;
    }

    /**
     * Gets the optin payload if available.
     * @return the Optin value as a FacebookOptin.
     */
    public FacebookOptin getOptin() {
        return this.optin;
    }

    /**
     * Sets the optin payload if available.
     * @param withOptin The Optin value.
     */
    public void setOptin(FacebookOptin withOptin) {
        this.optin = withOptin;
    }

}
