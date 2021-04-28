// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.multilingual.translation.model;

import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Translation result from Translator API v3.
 */
public class TranslatorResult {
    @JsonProperty("text")
    private String text;

    @JsonProperty("to")
    private String to;

    /**
     * Gets the translation result text.
     * @return Translation result.
     */
    public String getText() {
        return this.text;
    }

    /**
     * Sets the translation result text.
     * @param withText Translation result.
     */
    public void setText(String withText) {
        this.text = withText;
    }

    /**
     * Gets the target language locale.
     * @return Locale.
     */
    public String getTo() {
        return this.to;
    }

    /**
     * Sets the target language locale.
     * @param withTo Target locale.
     */
    public void setTo(String withTo) {
        this.to = withTo;
    }
}
