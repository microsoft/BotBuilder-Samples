// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.timex.resolution;

import com.microsoft.recognizers.datatypes.timex.expression.Constants.TimexTypes;
import com.microsoft.recognizers.datatypes.timex.expression.TimexProperty;

   /** The TimexProperty class takes a TIMEX expression as a string argument in its constructor.
    * This pulls all the component parts of the expression into properties on this object. You can
    * then manipulate the TIMEX expression via those properties.
    * The "Types" property infers a datetimeV2 type from the underlying set of properties.
    * If you take a TIMEX with date components and add time components you add the
    * inferred type datetime (its still a date).
    * Logic can be written against the inferred type, perhaps to have the bot ask the user for disambiguation.
    */
public final class Parsing {

    private Parsing() {
    }

    private static void describe(TimexProperty t) {
        System.out.print(t.getTimexValue() + " ");

        if (t.getTypes().contains(TimexTypes.DATE)) {
            if (t.getTypes().contains(TimexTypes.DEFINITE)) {
                System.out.print("We have a definite calendar date. ");
            } else {
                System.out.print("We have a date but there is some ambiguity. ");
            }
        }

        if (t.getTypes().contains(TimexTypes.TIME)) {
            System.out.print("We have a time.");
        }

        System.out.println();
    }

    /**
     * This method runs the parsing examples.
     */
    public static void examples() {
        describe(new TimexProperty("2017-05-29"));
        describe(new TimexProperty("XXXX-WXX-6"));
        describe(new TimexProperty("XXXX-WXX-6T16"));
        describe(new TimexProperty("T12"));
    }
}
