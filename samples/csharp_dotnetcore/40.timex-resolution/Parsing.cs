// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// The TimexProperty class takes a TIMEX expression as a string argument in its constructor.
    /// This pulls all the component parts of the expression into properties on this object. You can
    /// then manipulate the TIMEX expression via those properties.
    /// The "Types" property infers a datetimeV2 type from the underlying set of properties.
    /// If you take a TIMEX with date components and add time components you add the
    /// inferred type datetime (its still a date).
    /// Logic can be written against the inferred type, perhaps to have the bot ask the user for disamiguation.
    /// </summary>
    public static class Parsing
    {
        private static void Describe(TimexProperty t)
        {
            Console.Write($"{t.TimexValue} ");

            if (t.Types.Contains(Constants.TimexTypes.Date))
            {
                if (t.Types.Contains(Constants.TimexTypes.Definite))
                {
                    Console.Write("We have a definite calendar date. ");
                }
                else
                {
                    Console.Write("We have a date but there is some ambiguity. ");
                }
            }

            if (t.Types.Contains(Constants.TimexTypes.Time))
            {
                Console.Write("We have a time.");
            }

            Console.WriteLine();
        }

        public static void Examples()
        {
            Describe(new TimexProperty("2017-05-29"));
            Describe(new TimexProperty("XXXX-WXX-6"));
            Describe(new TimexProperty("XXXX-WXX-6T16"));
            Describe(new TimexProperty("T12"));
        }
    }
}
