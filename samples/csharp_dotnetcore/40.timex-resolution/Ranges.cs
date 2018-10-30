// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;

namespace Microsoft.BotBuilderSamples
{
    public static class Ranges
    {
        /// <summary>
        /// TIMEX expressions can represent date and time ranges. Here are a couple of examples.
        /// </summary>
        public static void DateRange()
        {
            // Run the recognizer.
            var results = DateTimeRecognizer.RecognizeDateTime("Some time in the next two weeks.", Culture.English);

            // We should find a single result in this example.
            foreach (var result in results)
            {
                // The resolution includes a single value because there is no ambiguity.
                var distinctTimexExpressions = new HashSet<string>();
                var values = (List<Dictionary<string, string>>)result.Resolution["values"];
                foreach (var value in values)
                {
                    // We are interested in the distinct set of TIMEX expressions.
                    if (value.TryGetValue("timex", out var timex))
                    {
                        distinctTimexExpressions.Add(timex);
                    }
                }

                // The TIMEX expression can also capture the notion of range.
                Console.WriteLine($"{result.Text} ( {string.Join(',', distinctTimexExpressions)} )");
            }
        }

        public static void TimeRange()
        {
            // Run the recognizer.
            var results = DateTimeRecognizer.RecognizeDateTime("Some time between 6pm and 6:30pm.", Culture.English);

            // We should find a single result in this example.
            foreach (var result in results)
            {
                // The resolution includes a single value because there is no ambiguity.
                var distinctTimexExpressions = new HashSet<string>();
                var values = (List<Dictionary<string, string>>)result.Resolution["values"];
                foreach (var value in values)
                {
                    // We are interested in the distinct set of TIMEX expressions.
                    if (value.TryGetValue("timex", out var timex))
                    {
                        distinctTimexExpressions.Add(timex);
                    }
                }

                // The TIMEX expression can also capture the notion of range.
                Console.WriteLine($"{result.Text} ( {string.Join(',', distinctTimexExpressions)} )");
            }
        }
    }
}
