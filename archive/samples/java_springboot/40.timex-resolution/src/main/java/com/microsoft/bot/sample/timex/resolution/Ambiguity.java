// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.timex.resolution;

import com.microsoft.recognizers.text.Culture;
import com.microsoft.recognizers.text.ModelResult;
import com.microsoft.recognizers.text.datetime.DateTimeRecognizer;

import java.util.LinkedHashSet;
import java.util.List;
import java.util.Map;

/**
 * TIMEX expressions are designed to represent ambiguous rather than definite dates.
 * For example:
 * "Monday" could be any Monday ever.
 * "May 5th" could be any one of the possible May 5th in the past or the future.
 * TIMEX does not represent ambiguous times. So if the natural language mentioned 4 o'clock
 * it could be either 4AM or 4PM. For that the recognizer (and by extension LUIS) would return two TIMEX expressions.
 * A TIMEX expression can include a date and time parts. So ambiguity of date can be combined with multiple results.
 * Code that deals with TIMEX expressions is frequently dealing with sets of TIMEX expressions.
 */
public final class Ambiguity {

    private Ambiguity() {
    }

    /**
     * This method avoid ambiguity obtaining 2 values, backwards and forwards in the calendar.
     */
    public static void dateAmbiguity() {
        // Run the recognizer.
        List<ModelResult> results = DateTimeRecognizer.recognizeDateTime(
            "Either Saturday or Sunday would work.",
            Culture.English);

        // We should find two results in this example.
        for (ModelResult result : results) {
            // The resolution includes two example values: going backwards and forwards from NOW in the calendar.
            LinkedHashSet<String> distinctTimexExpressions = new LinkedHashSet<>();
            List<Map<String, String>> values = (List<Map<String, String>>) result.resolution.get("values");
            for (Map<String, String> value : values) {
                // Each result includes a TIMEX expression that captures the inherent date but not time ambiguity.
                // We are interested in the distinct set of TIMEX expressions.
                String timex = value.get("timex");
                if (timex != null) {
                    distinctTimexExpressions.add(timex);
                }

                // There is also either a "value" property on each value or "start" and "end".
                // If you use ToString() on a TimeProperty object you will get same "value".
            }

            // The TIMEX expression captures date ambiguity so there will be a single distinct
            // expression for each result.
            String output = String.format("%s ( %s )", result.text, String.join(",", distinctTimexExpressions));
            System.out.println(output);

            // The result also includes a reference to the original string
            // but note the start and end index are both inclusive.
        }
    }

    /**
     * This method avoid ambiguity obtaining 2 values, one for AM and one for PM.
     */
    public static void timeAmbiguity() {
        // Run the recognizer.
        List<ModelResult> results = DateTimeRecognizer.recognizeDateTime(
            "We would like to arrive at 4 o'clock or 5 o'clock.",
            Culture.English);

        // We should find two results in this example.
        for (ModelResult result : results) {
            // The resolution includes two example values: one for AM and one for PM.
            LinkedHashSet<String> distinctTimexExpressions = new LinkedHashSet<>();
            List<Map<String, String>> values = (List<Map<String, String>>) result.resolution.get("values");
            for (Map<String, String> value : values) {
                // Each result includes a TIMEX expression that captures the inherent date but not time ambiguity.
                // We are interested in the distinct set of TIMEX expressions.
                String timex = value.get("timex");
                if (timex != null) {
                    distinctTimexExpressions.add(timex);
                }
            }

            // TIMEX expressions don't capture time ambiguity so there will be two distinct expressions for each result.
            String output = String.format("%s ( %s )", result.text, String.join(",", distinctTimexExpressions));
            System.out.println(output);
        }
    }

    /**
     * This method avoid ambiguity obtaining 4 different values,
     * backwards and forwards in the calendar and then AM and PM.
     */
    public static void dateTimeAmbiguity() {
        // Run the recognizer.
        List<ModelResult> results = DateTimeRecognizer.recognizeDateTime(
            "It will be ready Wednesday at 5 o'clock.",
            Culture.English);

        // We should find a single result in this example.
        for (ModelResult result : results) {
            // The resolution includes four example values: backwards and forward in the calendar and then AM and PM.
            LinkedHashSet<String> distinctTimexExpressions = new LinkedHashSet<>();
            List<Map<String, String>> values = (List<Map<String, String>>) result.resolution.get("values");
            for (Map<String, String> value : values) {
                // Each result includes a TIMEX expression that captures the inherent date but not time ambiguity.
                // We are interested in the distinct set of TIMEX expressions.
                String timex = value.get("timex");
                if (timex != null) {
                    distinctTimexExpressions.add(timex);
                }
            }

            // TIMEX expressions don't capture time ambiguity so there will be two distinct expressions for each result.
            String output = String.format("%s ( %s )", result.text, String.join(",", distinctTimexExpressions));
            System.out.println(output);
        }
    }
}
