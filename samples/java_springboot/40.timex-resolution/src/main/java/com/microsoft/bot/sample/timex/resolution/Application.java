// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.timex.resolution;

/**
 * This is the main program class.
 */
public final class Application {

    private Application() {
    }

    /**
     * This is the entry point method.
     * @param args String array to capture any command line parameters.
     */
    public static void main(String[] args) {
        // Creating TIMEX expressions from natural language using the Recognizer package.
        Ambiguity.dateAmbiguity();
        Ambiguity.timeAmbiguity();
        Ambiguity.dateTimeAmbiguity();
        Ranges.dateRange();
        Ranges.timeRange();

        // Manipulating TIMEX expressions in code using the TIMEX Datatype package.
        Parsing.examples();
        LanguageGeneration.examples();
        Resolutions.examples();
        Constraints.examples();
    }
}
