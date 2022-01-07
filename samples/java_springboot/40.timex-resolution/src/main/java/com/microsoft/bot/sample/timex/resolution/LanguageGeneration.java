// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.timex.resolution;

import com.microsoft.recognizers.datatypes.timex.expression.TimexProperty;

import java.time.LocalDateTime;

/**
 * This language generation capabilities are the logical opposite of what the recognizer does.
 * As an experiment try feeding the result of language generation back into a recognizer.
 * You should get back the same TIMEX expression in the result.
 */
public final class LanguageGeneration {

    private LanguageGeneration() {
    }

    private static void describe(TimexProperty t) {
        // Note natural language is often relative,
        // for example the sentence "Yesterday all my troubles seemed so far away."
        // Having your bot say something like "next Wednesday" in a response can make it sound more natural.
        LocalDateTime referenceDate = LocalDateTime.now();
        String output = String.format("%s %s", t.getTimexValue(), t.toNaturalLanguage(referenceDate));
        System.out.println(output);
    }

    /**
     * This method runs the resolver examples.
     */
    public static void examples() {
        describe(new TimexProperty("2019-05-29"));
        describe(new TimexProperty("XXXX-WXX-6"));
        describe(new TimexProperty("XXXX-WXX-6T16"));
        describe(new TimexProperty("T12"));
        describe(TimexProperty.fromDate(LocalDateTime.now()));
        describe(TimexProperty.fromDate(LocalDateTime.now().plusDays(1)));
    }
}
