// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Recognizers.Text.DateTime;
using Microsoft.Recognizers.Text;
using System.Collections.Generic;
using System;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// TIMEX expressions are designed to represent ambiguous rather than definite dates.
    /// For example:
    /// "Monday" could be any Monday ever.
    /// "May 5th" could be any one of the possible May 5th in the past or the future.
    /// TIMEX does not represent ambiguous times. So if the natural language mentioned 4 o'clock
    /// it could be either 4AM or 4PM. For that the recognizer (and by extension LUIS) would return two TIMEX expressions.
    /// A TIMEX expression can include a date and time parts. So ambiguity of date can be combined with multiple results.
    /// Code that deals with TIMEX expressions is frequently dealing with sets of TIMEX expressions.
    /// </summary>
    public static class Ambiguity
    {
        public static void DateAmbiguity()
        {
            // Run the recognizer.
            var results = DateTimeRecognizer.RecognizeDateTime("Either Saturday or Sunday would work.", Culture.English);

            // We should find two results in this example.
            foreach (var result in results)
            {
                // The resolution includes two example values: going backwards and forwards from NOW in the calendar.
                var distinctTimexExpressions = new HashSet<string>();
                var values = (List<Dictionary<string, string>>)result.Resolution["values"];
                foreach (var value in values)
                {
                    // Each result includes a TIMEX expression that captures the inherent date but not time ambiguity.
                    // We are interested in the distinct set of TIMEX expressions.
                    if (value.TryGetValue("timex", out var timex))
                    {
                        distinctTimexExpressions.Add(timex);
                    }

                    // There is also either a "value" property on each value or "start" and "end".
                    // If you use ToString() on a TimeProperty object you will get same "value".  
                }

                // The TIMEX expression captures date ambiguity so there will be a single distinct expression for each result.
                Console.WriteLine($"{result.Text} ( {string.Join(',', distinctTimexExpressions)} )");

                // The result also includes a reference to the original string - but note the start and end index are both inclusive.
            }
        }

        public static void TimeAmbiguity()
        {
            // Run the recognizer.
            var results = DateTimeRecognizer.RecognizeDateTime("We would like to arrive at 4 o'clock or 5 o'clock.", Culture.English);

            // We should find two results in this example.
            foreach (var result in results)
            {
                // The resolution includes two example values: one for AM and one for PM.
                var distinctTimexExpressions = new HashSet<string>();
                var values = (List<Dictionary<string, string>>)result.Resolution["values"];
                foreach (var value in values)
                {
                    // Each result includes a TIMEX expression that captures the inherent date but not time ambiguity.
                    // We are interested in the distinct set of TIMEX expressions.
                    if (value.TryGetValue("timex", out var timex))
                    {
                        distinctTimexExpressions.Add(timex);
                    }
                }

                // TIMEX expressions don't capture time ambiguity so there will be two distinct expressions for each result.
                Console.WriteLine($"{result.Text} ( {string.Join(',', distinctTimexExpressions)} )");
            }
        }

        public static void DateTimeAmbiguity()
        {
            // Run the recognizer.
            var results = DateTimeRecognizer.RecognizeDateTime("It will be ready Wednesday at 5 o'clock.", Culture.English);

            // We should find a single result in this example.
            foreach (var result in results)
            {
                // The resolution includes four example values: backwards and forward in the calendar and then AM and PM.
                var distinctTimexExpressions = new HashSet<string>();
                var values = (List<Dictionary<string, string>>)result.Resolution["values"];
                foreach (var value in values)
                {
                    // Each result includes a TIMEX expression that captures the inherent date but not time ambiguity.
                    // We are interested in the distinct set of TIMEX expressions.
                    if (value.TryGetValue("timex", out var timex))
                    {
                        distinctTimexExpressions.Add(timex);
                    }
                }

                // TIMEX expressions don't capture time ambiguity so there will be two distinct expressions for each result.
                Console.WriteLine($"{result.Text} ( {string.Join(',', distinctTimexExpressions)} )");
            }
        }
    }
}
