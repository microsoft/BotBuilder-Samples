// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This langauge generation capabilitis are the logical opposite of what the recognizer does.
    /// As an experiment try feeding the result of lanaguage generation back into a recognizer.
    /// You should get back the same TIMEX expression in the result.
    /// </summary>
    public static class LanguageGeneration
    {
        private static void Describe(TimexProperty t)
        {
            // Note natural language is often relative, for example the senstence "Yesterday all my troubles seemed so far away."
            // Having your bot say something like "next Wednesday" in a response can make it sound more natural.
            var referenceDate = DateTime.Now;
            Console.WriteLine($"{t.TimexValue} {t.ToNaturalLanguage(referenceDate)}");
        }

        public static void Examples()
        {
            Describe(new TimexProperty("2019-05-29"));
            Describe(new TimexProperty("XXXX-WXX-6"));
            Describe(new TimexProperty("XXXX-WXX-6T16"));
            Describe(new TimexProperty("T12"));
            Describe(TimexProperty.FromDate(DateTime.Now));
            Describe(TimexProperty.FromDate(DateTime.Now + TimeSpan.FromDays(1)));
        }
    }
}
