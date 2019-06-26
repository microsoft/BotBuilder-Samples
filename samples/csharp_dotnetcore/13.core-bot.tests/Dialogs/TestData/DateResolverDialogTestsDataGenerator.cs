// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Bot.Builder.Testing.XUnit;

namespace CoreBot.Tests.Dialogs.TestData
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Ignoring to make code more readable")]

    public class DateResolverDialogTestsDataGenerator
    {
        public static IEnumerable<object[]> DateResolverCases()
        {
            yield return BuildTestCaseObject(
                "tomorrow",
                null,
                $"{DateTime.Now.AddDays(1):yyyy-MM-dd}",
                new[,]
                {
                    { "hi", "When would you like to travel?" },
                    { "tomorrow", null },
                });

            yield return BuildTestCaseObject(
                "the day after tomorrow",
                null,
                $"{DateTime.Now.AddDays(2):yyyy-MM-dd}",
                new[,]
                {
                    { "hi", "When would you like to travel?" },
                    { "the day after tomorrow", null },
                });

            yield return BuildTestCaseObject(
                "two days from now",
                null,
                $"{DateTime.Now.AddDays(2):yyyy-MM-dd}",
                new[,]
                {
                    { "hi", "When would you like to travel?" },
                    { "two days from now", null },
                });

            yield return BuildTestCaseObject(
                "valid input given (tomorrow)",
                $"{DateTime.Now.AddDays(1):yyyy-MM-dd}",
                $"{DateTime.Now.AddDays(1):yyyy-MM-dd}",
                new[,]
                {
                    { "hi", null },
                });

            yield return BuildTestCaseObject(
                "retry prompt",
                null,
                $"{DateTime.Now.AddDays(1):yyyy-MM-dd}",
                new[,]
                {
                    { "hi", "When would you like to travel?" },
                    { "bananas", "I'm sorry, to make your booking please enter a full travel date including Day Month and Year." },
                    { "tomorrow", null },
                });

            yield return BuildTestCaseObject(
                "fuzzy time ",
                null,
                $"2055-05-05",
                new[,]
                {
                    { "hi", "When would you like to travel?" },
                    { "may 5th", "I'm sorry, to make your booking please enter a full travel date including Day Month and Year." },
                    { "may 5th 2055", null },
                });
        }

        private static object[] BuildTestCaseObject(string testCaseName, string input, string result, string[,] utterancesAndReplies)
        {
            var testData = new DateResolverDialogTestCase
            {
                Name = testCaseName,
                InitialData = input,
                ExpectedResult = result,
                UtterancesAndReplies = utterancesAndReplies,
            };
            return new object[] { new TestDataObject(testData) };
        }
    }
}
