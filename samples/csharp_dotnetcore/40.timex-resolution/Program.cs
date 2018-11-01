// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            Ambiguity.DateAmbiguity();
            Ambiguity.TimeAmbiguity();
            Ambiguity.DateTimeAmbiguity();
            Ranges.DateRange();
            Ranges.TimeRange();
            Parsing.Examples();
            LanguageGeneration.Examples();
            Resolution.Examples();
            Constraints.Examples();
        }
    }
}
