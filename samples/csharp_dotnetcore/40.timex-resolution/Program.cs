// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Microsoft.BotBuilderSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            var samples = new List<Action>
            {
                // Creating TIMEX expressions from natural language using the Recognizer package.
                Ambiguity.DateAmbiguity,
                Ambiguity.TimeAmbiguity,
                Ambiguity.DateTimeAmbiguity,
                Ranges.DateRange,
                Ranges.TimeRange,

                // Manipulating TIMEX expressions in code using the TIMEX Datatype package.
                Parsing.Examples,
                LanguageGeneration.Examples,
                Resolution.Examples,
                Constraints.Examples
            };

            foreach (var sample in samples)
            {
                Console.WriteLine(sample.Method.Name);
                sample();
                Console.WriteLine();
            }
        }
    }
}
