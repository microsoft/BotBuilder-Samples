// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.timex.resolution;

import com.microsoft.recognizers.text.Culture;
import com.microsoft.recognizers.text.ModelResult;
import com.microsoft.recognizers.text.datetime.DateTimeRecognizer;
import java.util.HashMap;
import java.util.LinkedHashSet;
import java.util.List;

/**
 * Class with date and time ranges examples.
 */
public final class Ranges {

    private Ranges() {
    }

    /**
     * TIMEX expressions can represent date and time ranges. Here are a couple of examples.
     */
    public static void dateRange() {
        // Run the recognizer.
        List<ModelResult> results =
                DateTimeRecognizer.recognizeDateTime("Some time in the next two weeks.",
                Culture.English);

        // We should find a single result in this example.
        for (ModelResult result : results) {
            // The resolution includes a single value because there is no ambiguity.
            LinkedHashSet<String> distinctTimexExpressions = new LinkedHashSet<>();
            List<HashMap<String, String>> values = (List<HashMap<String, String>>) result.resolution.get("values");
            for (HashMap<String, String> value : values) {
                // We are interested in the distinct set of TIMEX expressions.
                String timex = value.get("timex");
                if (timex != null) {
                    distinctTimexExpressions.add(timex);
                }
            }

            // The TIMEX expression can also capture the notion of range.
            String output = String.format("%s ( %s )", result.text, String.join(",", distinctTimexExpressions));
            System.out.println(output);
        }
    }

    /**
     * This method has examples of time ranges.
     */
    public static void timeRange() {
        // Run the recognizer.
        List<ModelResult> results =
                DateTimeRecognizer.recognizeDateTime("Some time between 6pm and 6:30pm.",
                Culture.English);

        // We should find a single result in this example.
        for (ModelResult result : results) {
            // The resolution includes a single value because there is no ambiguity.
            LinkedHashSet<String> distinctTimexExpressions = new LinkedHashSet<>();
            List<HashMap<String, String>> values = (List<HashMap<String, String>>) result.resolution.get("values");
            for (HashMap<String, String> value : values) {
                // We are interested in the distinct set of TIMEX expressions.
                String timex = value.get("timex");
                if (timex != null) {
                    distinctTimexExpressions.add(timex);
                }
            }

            // The TIMEX expression can also capture the notion of range.
            String output = String.format("%s ( %s )", result.text, String.join(",", distinctTimexExpressions));
            System.out.println(output);
        }
    }
}
