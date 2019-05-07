// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// The TimexRangeResolved can be used in application logic to apply constraints to a set of TIMEX expressions.
    /// The constraints themselves are TIMEX expressions. This is designed to appear a little like a database join,
    /// of course its a little less generic than that because dates can be complicated things.
    /// </summary>
    public static class Constraints
    {
        public static void Examples()
        {
            // When you give the recognizer the text "Wednesday 4 o'clock" you get these distinct TIMEX values back.

            // But our bot logic knows that whatever the user says it should be evaluated against the constraints of
            // a week from today with respect to the date part and in the evening with respect to the time part.

            var resolutions = TimexRangeResolver.Evaluate(
                new[] { "XXXX-WXX-3T04", "XXXX-WXX-3T16" },
                new[] { TimexCreator.WeekFromToday(), TimexCreator.Evening });

            var today = DateTime.Now;
            foreach (var resolution in resolutions)
            {
                Console.WriteLine(resolution.ToNaturalLanguage(today));
            }
        }
    }
}
