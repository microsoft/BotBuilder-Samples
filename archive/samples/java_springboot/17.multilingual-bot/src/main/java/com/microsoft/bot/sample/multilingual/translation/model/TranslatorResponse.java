// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.multilingual.translation.model;

import java.util.List;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

/**
 * Array of translated results from Translator API v3.
 */
@JsonIgnoreProperties(ignoreUnknown = true)
public class TranslatorResponse {
    @JsonProperty("translations")
    private List<TranslatorResult> translations;

    /**
     * Gets the translation results.
     * @return A list of {@link TranslatorResult}
     */
    public List<TranslatorResult> getTranslations() {
        return this.translations;
    }

    /**
     * Sets the translation results.
     * @param withTranslations A list of {@link TranslatorResult}
     */
    public void setTranslations(List<TranslatorResult> withTranslations) {
        this.translations = withTranslations;
    }
}
