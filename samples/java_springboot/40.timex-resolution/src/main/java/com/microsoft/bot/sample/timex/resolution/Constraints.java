// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

package com.microsoft.bot.sample.timex.resolution;

import com.microsoft.recognizers.datatypes.timex.expression.TimexCreator;
import com.microsoft.recognizers.datatypes.timex.expression.TimexProperty;
import com.microsoft.recognizers.datatypes.timex.expression.TimexRangeResolver;
import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashSet;
import java.util.List;

/**
 * The TimexRangeResolved can be used in application logic to apply constraints to a set of TIMEX expressions.
 * The constraints themselves are TIMEX expressions. This is designed to appear a little like a database join,
 * of course its a little less generic than that because dates can be complicated things.
 */
public final class Constraints {

    private Constraints() {
    }

    /**
     * This method runs the resolver examples.
     */
    public static void examples() {
        // When you give the recognizer the text "Wednesday 4 o'clock" you get these distinct TIMEX values back.

        // But our bot logic knows that whatever the user says it should be evaluated against the constraints of
        // a week from today with respect to the date part and in the evening with respect to the time part.

        List<TimexProperty> resolutions = TimexRangeResolver.evaluate(
                new HashSet<String>(Arrays.asList("XXXX-WXX-3T04", "XXXX-WXX-3T16")),
                new ArrayList<String>(Arrays.asList(TimexCreator.weekFromToday(null), TimexCreator.EVENING)));

        LocalDateTime today = LocalDateTime.now();
        for (TimexProperty resolution : resolutions) {
            System.out.println(resolution.toNaturalLanguage(today));
        }
    }
}
