// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MT License.


package com.microsoft.bot.sample.facebookevents.facebookmodel;

import com.fasterxml.jackson.annotation.JsonInclude;
import com.fasterxml.jackson.annotation.JsonProperty;

public class FacebookMessage {

    @JsonProperty(value = "mid")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private String messageId;

    @JsonProperty(value = "text")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private String text;

    @JsonProperty(value = "is_echo")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private boolean isEcho;

    @JsonProperty(value = "quick_reply")
    @JsonInclude(JsonInclude.Include.NON_EMPTY)
    private FacebookQuickReply quickReply;

    /**
     * Gets the message Id from Facebook.
     * @return the MessageId value as a String.
     */
    public String getMessageId() {
        return this.messageId;
    }

    /**
     * Sets the message Id from Facebook.
     * @param withMessageId The MessageId value.
     */
    public void setMessageId(String withMessageId) {
        this.messageId = withMessageId;
    }

    /**
     * Gets the message text.
     * @return the Text value as a String.
     */
    public String getText() {
        return this.text;
    }

    /**
     * Sets the message text.
     * @param withText The Text value.
     */
    public void setText(String withText) {
        this.text = withText;
    }

    /**
     * Gets whether the message is an echo message. See
     * {@link
     * https://developers#facebook#com/docs/messenger-platform/reference/webhook-events/message-echoes/} Echo Message
     * @return the IsEcho value as a boolean.
     */
    public boolean getIsEcho() {
        return this.isEcho;
    }

    /**
     * Sets whether the message is an echo message. See
     * {@link
     * https://developers#facebook#com/docs/messenger-platform/reference/webhook-events/message-echoes/} Echo Message
     * @param withIsEcho The IsEcho value.
     */
    public void setIsEcho(boolean withIsEcho) {
        this.isEcho = withIsEcho;
    }

    /**
     * Gets the quick reply.
     * @return the QuickReply value as a FacebookQuickReply.
     */
    public FacebookQuickReply getQuickReply() {
        return this.quickReply;
    }

    /**
     * Sets the quick reply.
     * @param withQuickReply The QuickReply value.
     */
    public void setQuickReply(FacebookQuickReply withQuickReply) {
        this.quickReply = withQuickReply;
    }

}

