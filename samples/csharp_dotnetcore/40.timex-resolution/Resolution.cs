// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Given the TIMEX expressions it is easy to create the computed example values that the recognizer gives.
    /// </summary>
    public static class Resolution
    {
        public static void Examples()
        {
            // When you give the recognzier the text "Wednesday 4 o'clock" you get these distinct TIMEX values back.

            var today = DateTime.Now;
            var resolution = TimexResolver.Resolve(new[] { "XXXX-WXX-3T04", "XXXX-WXX-3T16" }, today);

            Console.WriteLine(resolution.Values.Count);

            Console.WriteLine(resolution.Values[0].Timex);
            Console.WriteLine(resolution.Values[0].Type);
            Console.WriteLine(resolution.Values[0].Value);

            Console.WriteLine(resolution.Values[1].Timex);
            Console.WriteLine(resolution.Values[1].Type);
            Console.WriteLine(resolution.Values[1].Value);

            Console.WriteLine(resolution.Values[2].Timex);
            Console.WriteLine(resolution.Values[2].Type);
            Console.WriteLine(resolution.Values[2].Value);

            Console.WriteLine(resolution.Values[3].Timex);
            Console.WriteLine(resolution.Values[3].Type);
            Console.WriteLine(resolution.Values[3].Value);
        }
    }
}
